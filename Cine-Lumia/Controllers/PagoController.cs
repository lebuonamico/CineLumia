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

            // 🔥 CÁLCULOS DE CARGOS
            decimal totalSnacks = snacks.Sum(s => s.Precio * s.Cantidad);
            decimal cargoEntradas = 1600; // ejemplo
            decimal cargoSnacks = 700;    // ejemplo
            decimal subtotalEntradas = decimal.Parse(TempData["TotalCompra"]!.ToString()!, CultureInfo.InvariantCulture);
            ViewBag.CargoEntradas = cargoEntradas;
            ViewBag.CargoSnacks = snacks.Any() ? cargoSnacks : 0; // ⚡ solo si hay snacks
            ViewBag.TotalSnacks = totalSnacks;
            ViewBag.SubtotalEntradas = subtotalEntradas;
            // Model.TotalCompra = subtotal (entradas + snacks)
            vm.TotalCompra += totalSnacks; // aquí no sumamos cargos


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
            List<CarritoItemViewModel> snacks = new();
            if (!string.IsNullOrEmpty(carritoJson))
            {
                var snacksDeserializados = JsonSerializer.Deserialize<List<CarritoItemViewModel>>(carritoJson) ?? new List<CarritoItemViewModel>();
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
            }

            // --- CÁLCULO DE TOTALES ---
            decimal totalSnacks = snacks.Sum(s => s.Precio * s.Cantidad);
            decimal cargoSnacks = snacks.Any() ? 700 : 0; // Ejemplo de cargo

            var vm = new ResumenCompraViewModel
            {
                Proyeccion = proyeccion,
                AsientosSeleccionados = asientos,
                CantidadEntradas = cantidad,
                FormatoEntrada = formato,
                Snacks = snacks, // <-- Pasar snacks limpios al ViewModel
                CargoServicioSnacks = cargoSnacks,
                TotalCompra = totalEntradas + totalSnacks // <-- Calcular total final
            };

            TempData.Keep();
            TempData.Keep("PagoData");
            TempData.Keep("Asientos");
            TempData.Keep("IdProyeccion");

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

            return RedirectToAction("Index", "Consumibles");
        }

        [HttpGet]
        public IActionResult VolverConsumibles()
        {
            TempData.Keep("IdProyeccion");
            TempData.Keep("Asientos");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("PagoData");

            TempData["Modo"] = "Asientos";

            return RedirectToAction("Index", "Consumibles");
        }

    }
}
