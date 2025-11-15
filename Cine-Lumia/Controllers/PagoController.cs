using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace Cine_Lumia.Controllers
{
    public class PagoController : Controller
    {
        private readonly CineDbContext _context;

        public PagoController(CineDbContext context)
        {
            _context = context;
        }

        // ===========================================================
        //                          GET
        // ===========================================================
        [HttpGet]
        public IActionResult Index()
        {
            TempData.Keep("PagoData");  // <<< NECESARIO PARA NO PERDER DATOS

            if (TempData["IdProyeccion"] == null)
                return RedirectToAction("Index", "Home");

            int idProy = int.Parse(TempData["IdProyeccion"].ToString()!);

            var proy = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .Include(p => p.Sala).ThenInclude(s => s.Asientos)
                .FirstOrDefault(p => p.Id_Proyeccion == idProy);

            if (proy == null)
                return RedirectToAction("Index", "Home");

            // Recuperar asientos seleccionados
            var idsAsientos = (TempData["Asientos"]?.ToString() ?? "")
                              .Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse)
                              .ToList();

            var asientosSeleccionados = _context.Asientos
                .Where(a => idsAsientos.Contains(a.Id_Asiento))
                .ToList();

            // Armado fecha
            var fecha = proy.Fecha;
            var hora = proy.Hora.ToString(@"hh\:mm");
            string textoFecha;

            if (fecha.Date == DateTime.Today)
                textoFecha = $"Hoy {fecha:dd/MM} · {hora}";
            else
            {
                var diaSemana = fecha.ToString("dddd", new CultureInfo("es-AR"));
                diaSemana = char.ToUpper(diaSemana[0]) + diaSemana.Substring(1);
                textoFecha = $"{diaSemana} {fecha:dd/MM} · {hora}";
            }

            // Crear VM
            var vm = new PagoViewModel
            {
                Proyeccion = proy,
                AsientosSeleccionados = asientosSeleccionados,
                CantidadEntradas = idsAsientos.Count,
                TotalCompra = decimal.Parse(TempData["TotalCompra"]!.ToString()!, CultureInfo.InvariantCulture),
                FormatoEntrada = TempData["FormatoEntrada"]!.ToString()!
            };

            // =================================================
            //    RESTAURAR DATOS DEL FORMULARIO SI EXISTEN
            // =================================================

            if (TempData.Peek("PagoData") is string pagoJson)
            {
                var pagoTemp = JsonSerializer.Deserialize<PagoViewModel>(pagoJson);
                if (pagoTemp != null)
                {
                    vm.Titular = pagoTemp.Titular;
                    vm.DNI = pagoTemp.DNI;
                    vm.NumeroTarjeta = pagoTemp.NumeroTarjeta;
                    vm.Vencimiento = pagoTemp.Vencimiento;
                    vm.CVV = pagoTemp.CVV;
                }
            }

            TempData["PosterUrl"] = proy.Pelicula.PosterUrl;
            TempData["PeliculaNombre"] = proy.Pelicula.Nombre;
            TempData["CineNombre"] = proy.Sala.Cine.Nombre;
            TempData["SalaTexto"] = "Sala " + proy.Sala.Id_Sala;
            TempData["FechaHoraTexto"] = textoFecha;

            // Mantener todo
            TempData.Keep();
            TempData.Keep("IdProyeccion");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("Asientos");
            TempData.Keep("PagoData");

            return View(vm);
        }



        // ===========================================================
        //                  POST – CONFIRMAR PAGO
        // ===========================================================
        [HttpPost]
        public IActionResult ConfirmarSeleccion(
            int idProyeccion,
            PagoViewModel model,
            int[] asientosSeleccionados)
        {
            // Guardar info del formulario
            TempData["PagoData"] = JsonSerializer.Serialize(model);

            if (TempData["CantidadEntradas"] == null ||
                TempData["TotalCompra"] == null ||
                TempData["FormatoEntrada"] == null)
                return RedirectToAction("Index", "Home");

            int cantidad = int.Parse(TempData["CantidadEntradas"].ToString()!);
            string formato = TempData["FormatoEntrada"].ToString()!;
            decimal total = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);

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

            TempData.Keep();
            TempData.Keep("PagoData");
            TempData.Keep("Asientos");
            TempData.Keep("IdProyeccion");

            return View("~/Views/ResumenCompra/Index.cshtml", vm);
        }



        // ===========================================================
        //                 BOTÓN VOLVER (desde PAGO)
        // ===========================================================
        [HttpPost]
        public IActionResult VolverSeleccion(PagoViewModel model)
        {
            TempData["PagoData"] = JsonSerializer.Serialize(model);

            TempData.Keep("IdProyeccion");
            TempData.Keep("Asientos");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("PagoData");

            return RedirectToAction("Seleccion", "Asientos");
        }
    }
}
