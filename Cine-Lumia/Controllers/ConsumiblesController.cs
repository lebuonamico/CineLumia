using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Linq;

namespace Cine_Lumia.Controllers
{
    public class ConsumiblesController : Controller
    {
        private readonly CineDbContext _context;

        public ConsumiblesController(CineDbContext context)
        {
            _context = context;
        }

        // ================================
        // LISTADO DE CONSUMIBLES POR CINE
        // ================================
        public IActionResult Index()
        {
            string modo = TempData["Modo"]?.ToString() ?? "Normal";

            ViewBag.Modo = modo;

            var selectedCineId = HttpContext.Session.GetInt32("CineSeleccionado");
            List<CineConsumible> consumiblesPorCine;

            if (!selectedCineId.HasValue)
            {
                var firstCine = _context.Cines.FirstOrDefault();
                if (firstCine != null)
                {
                    selectedCineId = firstCine.Id_Cine;
                    HttpContext.Session.SetInt32("CineSeleccionado", selectedCineId.Value);
                }
            }

            if (selectedCineId.HasValue)
            {
                consumiblesPorCine = _context.CineConsumibles
                    .Include(cc => cc.Cine)
                    .Include(cc => cc.Consumible)
                    .Where(cc => cc.Id_Cine == selectedCineId.Value)
                    .ToList();
            }
            else
            {
                consumiblesPorCine = new List<CineConsumible>();
            }

            // RESTAURAR CARRITO
            var carritoJson = HttpContext.Session.GetString("CarritoSnacks");
            if (carritoJson != null && modo == "Asientos")
            {
                ViewBag.CarritoSnacks = carritoJson;
            }

            // Resumen compra
            ViewBag.ResumenCompra = TempData["ResumenCompra"];

            // ⚠ IMPORTANTE: NO volver a tocar TempData["Modo"]
            TempData.Remove("Modo");

            return View(consumiblesPorCine);
        }



        // ================================
        // COMPRAR SNACK
        // ================================
        [HttpPost]
        [Authorize]
        public IActionResult Comprar([FromBody] CompraLoteViewModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return Unauthorized(new { mensaje = "⚠️ Debes iniciar sesión para poder comprar." });
            }

            var espectador = _context.Espectadores.FirstOrDefault(e => e.Email == userEmail);
            if (espectador == null)
            {
                return Unauthorized(new { mensaje = "⚠️ Usuario no encontrado. Inicia sesión nuevamente." });
            }

            var cine = _context.Cines.Find(model.CineId);
            if (cine == null)
            {
                return NotFound(new { mensaje = "❌ Cine no encontrado." });
            }

            // Usamos una transacción para asegurar que todas las operaciones se completen o ninguna lo haga.
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in model.Items)
                    {
                        var cineConsumible = _context.CineConsumibles
                            .FirstOrDefault(cc => cc.Id_Cine == model.CineId && cc.Id_Consumible == item.ConsumibleId);

                        if (cineConsumible == null)
                        {
                            var consumible = _context.Consumibles.Find(item.ConsumibleId);
                            var consumibleNombre = consumible?.Nombre ?? $"ID {item.ConsumibleId}";
                            return NotFound(new { mensaje = $"❌ Snack '{consumibleNombre}' no encontrado para el cine '{cine.Nombre}'." });
                        }

                        if (cineConsumible.Stock < item.Cantidad)
                        {
                            var consumible = _context.Consumibles.Find(item.ConsumibleId);
                            return BadRequest(new { mensaje = $"🚫 No hay suficiente stock de '{consumible?.Nombre}' para completar la compra." });
                        }

                        // Restar stock
                        cineConsumible.Stock -= item.Cantidad;

                        // Registrar la compra
                        var espectadorConsumible = new EspectadorConsumible
                        {
                            Id_Espectador = espectador.Id_Espectador,
                            Id_Consumible = item.ConsumibleId,
                            Id_Cine = model.CineId,
                            Cantidad = item.Cantidad,
                            Fecha = System.DateTime.Now
                        };
                        _context.EspectadorConsumibles.Add(espectadorConsumible);
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Si algo falla, revertimos todos los cambios.
                    transaction.Rollback();
                    return StatusCode(500, new { mensaje = "🚨 Ocurrió un error inesperado al procesar la compra. Por favor, inténtalo de nuevo." });
                }
            }


            // Generar resumen de compra actualizado
            GenerarResumenCompra(espectador.Id_Espectador);

            return Ok(new
            {
                mensaje = $"✅ Compra realizada con éxito en {cine.Nombre}."
            });
        }
        [HttpPost]
        public IActionResult FinalizarSnacks()
        {
            TempData["SnacksOK"] = "1";  // Marca de que pasaste por snacks
            TempData.Keep();

            return RedirectToAction("Index", "Pago");
        }
        [HttpPost]
        public IActionResult VolverAAsientos()
        {
            TempData.Keep(); // Mantiene asientos, entradas, total, etc.
            return RedirectToAction("Seleccion", "Asientos");
        }

        [HttpGet]
        public IActionResult EntradaSnaks()
        {
            TempData.Keep();
            // Verificamos que venimos del flujo correcto
            if (!TempData.ContainsKey("IdProyeccionSeleccionado"))
                return RedirectToAction("Index", "Home");

            int idProyeccion = int.Parse(TempData["IdProyeccionSeleccionado"].ToString());

            var proyeccion = _context.Proyecciones
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null)
                return RedirectToAction("Index", "Home");

            int cineId = proyeccion.Sala.Id_Cine;

            var snacks = _context.CineConsumibles
                .Include(cc => cc.Cine)
                .Include(cc => cc.Consumible)
                .Where(cc => cc.Id_Cine == cineId)
                .ToList();

            // Guardamos el cine por si el usuario vuelve
            TempData["CineSnacks"] = cineId;

            // Añadir el carrito existente al ViewBag para que la vista lo pueda restaurar
            var carritoJson = HttpContext.Session.GetString("CarritoSnacks");
            if (carritoJson != null)
            {
                ViewBag.CarritoSnacks = carritoJson;
            }

            ViewBag.Modo = "Asientos";
            TempData.Keep();
            return View("Index", snacks);
        }
        [HttpPost]
        public IActionResult GuardarCarrito([FromBody] List<CarritoItemViewModel> carritoRecibido)
        {
            if (carritoRecibido == null)
            {
                // Si el cliente envía null, limpiar el carrito para estar en un estado consistente.
                HttpContext.Session.Remove("CarritoSnacks");
                return Ok();
            }

            // El cliente es la única fuente de verdad. Consolidar la lista que envía.
            var carritoConsolidado = carritoRecibido
                .GroupBy(item => item.Id) // Agrupar por el ID del consumible
                .Select(grupo => new CarritoItemViewModel
                {
                    Id = grupo.Key,
                    Snack = grupo.First().Snack,
                    Cantidad = grupo.Sum(item => item.Cantidad), // Sumar cantidades si el cliente envía duplicados
                    Precio = grupo.First().Precio,
                    ImagenUrl = grupo.First().ImagenUrl,
                    CineId = grupo.First().CineId
                })
                .ToList();

            // Sobrescribir completamente la sesión con el nuevo estado del carrito.
            HttpContext.Session.SetString("CarritoSnacks", JsonSerializer.Serialize(carritoConsolidado));
            return Ok();
        }




        // =====================================
        // MÉTODO PRIVADO: GENERAR RESUMEN COMPRA
        // =====================================
        private void GenerarResumenCompra(int idEspectador)
        {
            var resumen = _context.EspectadorConsumibles
                .Include(ec => ec.Consumible)
                .Include(ec => ec.Cine)
                .Where(ec => ec.Id_Espectador == idEspectador)
                .AsEnumerable() // fuerza evaluación en memoria para evitar errores de traducción SQL
                .GroupBy(ec => new
                {
                    ConsumibleNombre = ec.Consumible?.Nombre ?? "Desconocido",
                    CineNombre = ec.Cine?.Nombre ?? "Sin cine"
                })
                .Select(g => new
                {
                    Consumible = g.Key.ConsumibleNombre,
                    Cine = g.Key.CineNombre,
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToList();

            if (resumen.Any())
            {
                string textoResumen = "🧾 Resumen de tus compras: ";
                textoResumen += string.Join(" | ", resumen.Select(r => $"{r.Cantidad}x {r.Consumible} ({r.Cine})"));
                TempData["ResumenCompra"] = textoResumen;
            }
        }
    }

    // =====================================
    // VIEWMODELS DE COMPRA
    // =====================================
    public class CompraLoteViewModel
    {
        public int CineId { get; set; }
        public List<ItemCompraViewModel> Items { get; set; } = new List<ItemCompraViewModel>();
    }

    public class ItemCompraViewModel
    {
        public int ConsumibleId { get; set; }
        public int Cantidad { get; set; }
    }
}