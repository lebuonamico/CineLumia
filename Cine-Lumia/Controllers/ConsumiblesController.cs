using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cine_Lumia.Entities;
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
        public IActionResult Comprar([FromBody] ComprarViewModel model)
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

            var cineConsumible = _context.CineConsumibles
                .Include(cc => cc.Consumible)
                .Include(cc => cc.Cine)
                .FirstOrDefault(cc => cc.Id_Cine == model.CineId && cc.Id_Consumible == model.ConsumibleId);

            if (cineConsumible == null)
            {
                return NotFound(new { mensaje = "❌ Snack no encontrado para este cine." });
            }

            if (cineConsumible.Stock < model.Cantidad)
            {
                return BadRequest(new { mensaje = "🚫 No hay suficiente stock disponible para completar la compra." });
            }

            // Restar stock
            cineConsumible.Stock -= model.Cantidad;

            // Registrar la compra
            var espectadorConsumible = new EspectadorConsumible
            {
                Id_Espectador = espectador.Id_Espectador,
                Id_Consumible = model.ConsumibleId,
                Id_Cine = model.CineId,
                Cantidad = model.Cantidad,
                Fecha = System.DateTime.Now
            };

            _context.EspectadorConsumibles.Add(espectadorConsumible);
            _context.SaveChanges();

            // Generar resumen de compra actualizado
            GenerarResumenCompra(espectador.Id_Espectador);

            return Ok(new
            {
                mensaje = $"✅ Compra realizada con éxito: {model.Cantidad}x {cineConsumible.Consumible.Nombre} en {cineConsumible.Cine.Nombre}.",
                nuevoStock = cineConsumible.Stock
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
    // VIEWMODEL DE COMPRA
    // =====================================
    public class ComprarViewModel
    {
        public int ConsumibleId { get; set; }
        public int CineId { get; set; }
        public int Cantidad { get; set; }
    }
}
