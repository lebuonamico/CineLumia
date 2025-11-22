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
            TempData.Keep("PagoData");

            var carritoJson = HttpContext.Session.GetString("CarritoSnacks");
            List<CarritoItemViewModel> snacks = new();

            if (!string.IsNullOrEmpty(carritoJson))
            {
                var snacksDeserializados = JsonSerializer.Deserialize<List<CarritoItemViewModel>>(carritoJson) ?? new List<CarritoItemViewModel>();

                // Consolidar la lista para asegurarse de que no haya duplicados, sin importar el estado de la sesión.
                snacks = snacksDeserializados
                    .GroupBy(s => s.Id)
                    .Select(g => new CarritoItemViewModel
                    {
                        Id = g.Key,
                        Snack = g.First().Snack,
                        Cantidad = g.Sum(item => item.Cantidad),
                        Precio = g.First().Precio,
                        ImagenUrl = g.First().ImagenUrl,
                        CineId = g.First().CineId
                    }).ToList();

                foreach (var snack in snacks)
                {
                    snack.ImagenUrl = _context.Consumibles
                        .Where(c => c.Id_Consumible == snack.Id)
                        .Select(c => c.PosterUrl)
                        .FirstOrDefault() ?? "/images/snack_placeholder.png";
                }
            }

            ViewBag.Snacks = snacks;

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

            // Obtener asientos seleccionados
            var idsAsientos = (TempData["Asientos"]?.ToString() ?? "")
                              .Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse)
                              .ToList();

            var asientosSeleccionados = _context.Asientos
                .Where(a => idsAsientos.Contains(a.Id_Asiento))
                .ToList();

            // Crear VM
            var vm = new PagoViewModel
            {
                Proyeccion = proy,
                AsientosSeleccionados = asientosSeleccionados,
                CantidadEntradas = idsAsientos.Count,
                TotalCompra = decimal.Parse(TempData["TotalCompra"]!.ToString()!, CultureInfo.InvariantCulture),
                FormatoEntrada = TempData["FormatoEntrada"]!.ToString()!
            };

            decimal totalSnacks = snacks.Sum(s => s.Precio * s.Cantidad);
            decimal subtotalEntradas = decimal.Parse(TempData["TotalCompra"]!.ToString()!, CultureInfo.InvariantCulture);

            decimal cargoEntradas = 0;
            decimal cargoSnacks = 0;

            // Aplica cargo por entradas solo si hay entradas
            if (subtotalEntradas > 0)
                cargoEntradas = 1600; // ejemplo

            // Aplica cargo por snacks solo si hay snacks
            if (snacks.Any())
                cargoSnacks = 700;

            decimal totalFinal = subtotalEntradas + totalSnacks + cargoEntradas + cargoSnacks;

            // Guardamos en TempData para Resumen
            TempData["TotalFinal"] = totalFinal.ToString(CultureInfo.InvariantCulture);


            // Pasamos info a la vista
            ViewBag.CargoEntradas = cargoEntradas;
            ViewBag.CargoSnacks = cargoSnacks;
            ViewBag.TotalSnacks = totalSnacks;
            ViewBag.SubtotalEntradas = subtotalEntradas;



            // Restaurar datos previos
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

            TempData.Keep();
            Console.WriteLine("===== DEBUG TOTAL COMPRA =====");
            Console.WriteLine($"TotalCompra inicial (TempData): {TempData["TotalCompra"]}");
            Console.WriteLine($"totalSnacks: {totalSnacks}");
            Console.WriteLine($"cargoSnacks: {cargoSnacks}");
            Console.WriteLine($"cargoEntradas: {cargoEntradas}");
            Console.WriteLine($"vm.TotalCompra final: {vm.TotalCompra}");
            Console.WriteLine("================================");

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

            // --- LECTURA DE DATOS DE ENTRADAS ---
            int cantidad = int.Parse(TempData["CantidadEntradas"].ToString()!);
            string formato = TempData["FormatoEntrada"].ToString()!;
            decimal totalEntradas = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);

            var proyeccion = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            var asientos = _context.Asientos
                .Where(a => asientosSeleccionados.Contains(a.Id_Asiento))
                .ToList();

            // --- LECTURA Y CONSOLIDACIÓN DE SNACKS ---
            var carritoJson = HttpContext.Session.GetString("CarritoSnacks");
            List<CarritoItemViewModel> snacksCarrito = new();
            List<SnackSeleccionadoViewModel> snacksParaResumen = new();

            if (!string.IsNullOrEmpty(carritoJson))
            {
                var snacksDeserializados = JsonSerializer.Deserialize<List<CarritoItemViewModel>>(carritoJson) ?? new List<CarritoItemViewModel>();
                snacksCarrito = snacksDeserializados
                    .GroupBy(s => s.Id)
                    .Select(g => new CarritoItemViewModel
                    {
                        Id = g.Key,
                        Snack = g.First().Snack,
                        Cantidad = g.Sum(item => item.Cantidad),
                        Precio = g.First().Precio,
                        ImagenUrl = g.First().ImagenUrl,
                        CineId = g.First().CineId
                    }).ToList();

                // Convertir a SnackSeleccionadoViewModel para TempData
                snacksParaResumen = snacksCarrito.Select(s => new SnackSeleccionadoViewModel
                {
                    IdConsumible = s.Id,
                    Nombre = s.Snack!,
                    Precio = s.Precio,
                    Cantidad = s.Cantidad,
                    IdCine = s.CineId // Asegurarse de que CineId se pasa correctamente
                }).ToList();
            }

            // Guardar snacks en TempData para que ResumenCompraController los recupere
            TempData["SnacksSeleccionados"] = System.Text.Json.JsonSerializer.Serialize(snacksParaResumen);

            // --- CÁLCULO DE TOTALES ---
            decimal totalSnacks = snacksCarrito.Sum(s => s.Precio * s.Cantidad);
            decimal totalCompra = 0;
            if (TempData["TotalFinal"] != null)
            {
                decimal.TryParse(TempData["TotalFinal"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out totalCompra);
            }

            var vm = new ResumenCompraViewModel
            {
                Proyeccion = proyeccion,
                AsientosSeleccionados = asientos,
                CantidadEntradas = cantidad,
                FormatoEntrada = formato,
                TotalCompra = totalCompra,
                SnacksSeleccionados = snacksParaResumen // También poblar para la vista directa
            };



            TempData.Keep();
            TempData.Keep("PagoData");
            TempData.Keep("Asientos");
            TempData.Keep("IdProyeccion");
            TempData.Keep("SnacksSeleccionados"); // Asegurar que persista para el POST posterior

            return View("~/Views/ResumenCompra/Index.cshtml", vm);
        }
        /*
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
        }*/
        [HttpPost]
        public IActionResult VolverConsumible(PagoViewModel model)
        {
            TempData["PagoData"] = JsonSerializer.Serialize(model);

            TempData.Keep("IdProyeccion");
            TempData.Keep("Asientos");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("PagoData");

            // Indicar modo
            TempData["Modo"] = "Asientos";
            TempData.Keep("Modo");
            TempData.Keep();

            return RedirectToAction("Index", "Consumibles");
        }
    }
}
