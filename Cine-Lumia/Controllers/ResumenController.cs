using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cine_Lumia.Controllers
{
    public class ResumenCompraController : Controller
    {
        private readonly CineDbContext _context;

        public ResumenCompraController(CineDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Confirmar(
            int IdProyeccion,
            int CantidadEntradas,
            decimal TotalCompra,
            string FormatoEntrada,
            List<int> AsientosSeleccionadosIds)
        {
            if (AsientosSeleccionadosIds == null || !AsientosSeleccionadosIds.Any())
                return BadRequest("No se seleccionaron asientos.");

            var proyeccion = await _context.Proyecciones
                .Include(p => p.Sala)
                .FirstOrDefaultAsync(p => p.Id_Proyeccion == IdProyeccion);

            if (proyeccion == null)
                return BadRequest("Proyección no encontrada.");

            // Buscar formato y tipo de entrada según el nombre recibido
            var formato = await _context.Formato.FirstOrDefaultAsync(f => f.Nombre == FormatoEntrada);
            var tipoEntrada = formato != null
                ? await _context.TipoEntrada.FirstOrDefaultAsync(t => t.Id_Formato == formato.Id_Formato)
                : await _context.TipoEntrada.FirstOrDefaultAsync();

            if (tipoEntrada == null)
                return BadRequest("Tipo de entrada no encontrado.");

            // Obtener espectador actual (usuario logueado)
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var espectador = await _context.Espectadores.FirstOrDefaultAsync(e => e.Email == userEmail);

            if (espectador == null)
                return BadRequest("No se encontró el espectador logueado.");

            try
            {
                foreach (var idAsiento in AsientosSeleccionadosIds)
                {
                    var asiento = await _context.Asientos.FindAsync(idAsiento);
                    if (asiento == null) continue;

                    asiento.Disponible = false;
                    _context.Asientos.Update(asiento);

                    var entrada = new Entrada
                    {
                        Id_Proyeccion = IdProyeccion,
                        Id_Asiento = asiento.Id_Asiento,
                        Id_Espectador = espectador.Id_Espectador,
                        Id_TipoEntrada = tipoEntrada.Id_TipoEntrada,
                        FechaCompra = DateTime.Now
                    };

                    _context.Entradas.Add(entrada);
                }

                await _context.SaveChangesAsync();

                TempData["CompraExitosa"] = "Compra realizada con éxito.";
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar entrada: {ex.Message}");
                return StatusCode(500, "Error interno al guardar la compra.");
            }
        }
    }
}
