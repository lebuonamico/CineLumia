using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Cine_Lumia.Controllers
{
    public class AsientosController : Controller
    {
        private readonly CineDbContext _context;
        public AsientosController(CineDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Index(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada)
        {
            TempData["CantidadEntradas"] = cantidadEntradas;
            TempData["FormatoEntrada"] = formatoEntrada;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);

            return RedirectToAction("Seleccion", new { idProyeccion });
        }
        [HttpGet]
        public IActionResult Seleccion(int idProyeccion)
        {
            var proyeccion = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Asientos)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .Include(p => p.Sala).ThenInclude(s => s.Formato)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            var ocupados = _context.Entradas
                .Where(e => e.Id_Proyeccion == idProyeccion)
                .Select(e => e.Id_Asiento)
                .ToList();

            var asientos = proyeccion.Sala.Asientos
                .Select(a => new Asiento
                {
                    Id_Asiento = a.Id_Asiento,
                    Fila = a.Fila,
                    Columna = a.Columna,
                    Disponible = !ocupados.Contains(a.Id_Asiento)
                }).ToList();

            var vm = new SeleccionAsientoViewModel
            {
                Id_Proyeccion = idProyeccion,
                Proyeccion = proyeccion,
                Asientos = asientos,
                Columnas = proyeccion.Sala.Cant_Butacas,
                Filas = proyeccion.Sala.Cant_Filas,
                CantidadEntradas = (int)TempData["CantidadEntradas"],
                FormatoEntrada = TempData["FormatoEntrada"]?.ToString(),

                TotalCompra = decimal.Parse(
                    TempData["TotalCompra"].ToString(),
                    CultureInfo.InvariantCulture
                )
            };

            return View("Index", vm);
        }




        [HttpPost]
        public IActionResult ConfirmarSeleccion(int[] asientosSeleccionados)
        {
            // Lógica para guardar entradas o pasar al pago
            // ...
            return RedirectToAction("ResumenCompra");
        }

    }
}
