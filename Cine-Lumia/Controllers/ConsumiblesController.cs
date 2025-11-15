using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cine_Lumia.Entities;
using System.Linq;
 // This line needs to be added

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
            var consumiblesPorCine = _context.CineConsumibles
                .Include(cc => cc.Cine)
                .Include(cc => cc.Consumible)
                .ToList();

            // Si hay un resumen de compra previo, lo pasamos a la vista
            ViewBag.ResumenCompra = TempData["ResumenCompra"];

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