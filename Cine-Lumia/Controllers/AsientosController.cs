using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Cine_Lumia.Controllers
{
    [Authorize]
    public class AsientosController : Controller
    {
        private readonly CineDbContext _context;
        public AsientosController(CineDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada)
        {
            TempData["CantidadEntradas"] = cantidadEntradas;
            TempData["FormatoEntrada"] = formatoEntrada;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);

            return RedirectToAction("Seleccion", new { idProyeccion });
        }
        [HttpGet]
        public IActionResult Seleccion(int idProyeccion, int? cantidadEntradas, decimal? totalCompra, string? formatoEntrada, string? asientos)
        {
            // ✅ Restaurar TempData si vienen parámetros
            if (cantidadEntradas.HasValue && totalCompra.HasValue && !string.IsNullOrEmpty(formatoEntrada))
            {
                TempData["CantidadEntradas"] = cantidadEntradas.Value;
                TempData["TotalCompra"] = totalCompra.Value.ToString(CultureInfo.InvariantCulture);
                TempData["FormatoEntrada"] = formatoEntrada;
            }

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

            var asientosSeleccionadosIds = new List<int>();
            if (!string.IsNullOrEmpty(asientos))
            {
                asientosSeleccionadosIds = asientos.Split(',').Select(int.Parse).ToList();
            }

            var asientosList = proyeccion.Sala.Asientos
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
                Asientos = asientosList,
                Columnas = proyeccion.Sala.Cant_Butacas,
                Filas = proyeccion.Sala.Cant_Filas,
                CantidadEntradas = (int)TempData["CantidadEntradas"],
                FormatoEntrada = TempData["FormatoEntrada"].ToString(),
                TotalCompra = decimal.Parse(TempData["TotalCompra"].ToString(), CultureInfo.InvariantCulture)
            };

            TempData.Keep();

            // ✅ Guardamos los seleccionados en ViewData para marcarlos en el front
            ViewData["AsientosSeleccionados"] = string.Join(",", asientosSeleccionadosIds);

            return View("Index", vm);
        }






        [HttpPost]
        public IActionResult ConfirmarSeleccion(int[] asientosSeleccionados)
        {
            // Recuperamos datos del TempData
            if (TempData["CantidadEntradas"] == null || TempData["TotalCompra"] == null || TempData["FormatoEntrada"] == null)
                return RedirectToAction("Index", "Home");

            int cantidad = int.Parse(TempData["CantidadEntradas"].ToString()!);
            string formato = TempData["FormatoEntrada"].ToString()!;
            decimal total = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);
            int idProyeccion = int.Parse(Request.Query["idProyeccion"]);

            var proyeccion = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            var asientos = _context.Asientos
                .Where(a => asientosSeleccionados.Contains(a.Id_Asiento))
                .ToList();

            var vm = new ResumenCompraViewModel
            {
                Proyeccion = proyeccion,
                AsientosSeleccionados = asientos,
                CantidadEntradas = cantidad,
                FormatoEntrada = formato,
                TotalCompra = total
            };

            // ✅ Mantener vivos los datos para cuando se haga "Volver"
            TempData.Keep();

            return View("~/Views/ResumenCompra/Index.cshtml", vm);
        }



    }
}
