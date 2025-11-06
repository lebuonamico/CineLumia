using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine_Lumia.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cine_Lumia.Entities;

namespace Cine_Lumia.Controllers
{
    public class ConsumiblesController : Controller
    {
        private readonly CineDbContext _context;

        public ConsumiblesController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var consumiblesPorCine = _context.CineConsumibles
                .Include(cc => cc.Cine)
                .Include(cc => cc.Consumible)
                .ToList();

            return View(consumiblesPorCine);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Comprar([FromBody] ComprarViewModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return Unauthorized();
            }

            var espectador = _context.Espectadores.FirstOrDefault(e => e.Email == userEmail);
            if (espectador == null)
            {
                return Unauthorized();
            }

            var cineConsumible = _context.CineConsumibles
                .FirstOrDefault(cc => cc.Id_Cine == model.CineId && cc.Id_Consumible == model.ConsumibleId);

            if (cineConsumible == null)
            {
                return NotFound("Snack no encontrado.");
            }

            if (cineConsumible.Stock < model.Cantidad)
            {
                return BadRequest("No hay suficiente stock disponible.");
            }

            cineConsumible.Stock -= model.Cantidad;

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

            return Ok(new { message = "Compra realizada con éxito" });
        }
    }

    public class ComprarViewModel
    {
        public int ConsumibleId { get; set; }
        public int CineId { get; set; }
        public int Cantidad { get; set; }
    }
}
