using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // Para Include, FirstOrDefaultAsync, etc.
using System.Security.Claims;
[Authorize]
public class ResumenCompraController : Controller
{
    private readonly CineDbContext _context;

    public ResumenCompraController(CineDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(ResumenCompraViewModel vm)
    {
        if (vm == null || vm.AsientosSeleccionados == null || !vm.AsientosSeleccionados.Any())
        {
            // Evitar acceso directo
            TempData["Error"] = "No hay datos de la compra.";
            return RedirectToAction("Index", "Home");
        }
        TempData.Keep("IdProyeccion");
        TempData.Keep("CantidadEntradas");
        TempData.Keep("TotalCompra");
        TempData.Keep("FormatoEntrada");
        TempData.Keep("Asientos");
        TempData.Keep("PagoData");

        // Guardar snacks seleccionados en TempData para que estén disponibles en el POST de Confirmar
        if (vm.SnacksSeleccionados != null && vm.SnacksSeleccionados.Any())
        {
            TempData["SnacksSeleccionados"] = System.Text.Json.JsonSerializer.Serialize(vm.SnacksSeleccionados);
            TempData.Keep("SnacksSeleccionados"); // Asegurar que persista
        }

        return View(vm);
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

        var formato = await _context.Formato.FirstOrDefaultAsync(f => f.Nombre == FormatoEntrada);
        var tipoEntrada = formato != null
            ? await _context.TipoEntrada.FirstOrDefaultAsync(t => t.Id_Formato == formato.Id_Formato)
            : await _context.TipoEntrada.FirstOrDefaultAsync();

        if (tipoEntrada == null)
            return BadRequest("Tipo de entrada no encontrado.");

        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var espectador = await _context.Espectadores.FirstOrDefaultAsync(e => e.Email == userEmail);

        if (espectador == null)
            return BadRequest("No se encontró el espectador logueado.");

        try
        {
            decimal precioPorEntrada = 0;
            if (CantidadEntradas > 0)
            {
                precioPorEntrada = TotalCompra / CantidadEntradas;
            }

            // Guardar entradas
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
                    FechaCompra = DateTime.Now,
                    PrecioTotal = precioPorEntrada // Asignar el precio total por entrada
                };

                _context.Entradas.Add(entrada);
            }

            // Guardar snacks (recuperar de TempData)
            string snacksJson = TempData["SnacksSeleccionados"] as string;

            if (!string.IsNullOrEmpty(snacksJson))
            {
                List<SnackSeleccionadoViewModel> snacksSeleccionados = null;
                try
                {
                    snacksSeleccionados = System.Text.Json.JsonSerializer.Deserialize<List<SnackSeleccionadoViewModel>>(snacksJson);
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    Console.WriteLine($"❌ Error de deserialización de snacks: {jsonEx.Message}"); // Keep this one for unexpected JSON issues
                }


                if (snacksSeleccionados != null && snacksSeleccionados.Any())
                {
                    foreach (var snack in snacksSeleccionados)
                    {
                        var espectadorConsumible = new EspectadorConsumible
                        {
                            Id_Espectador = espectador.Id_Espectador,
                            Id_Consumible = snack.IdConsumible,
                            Id_Cine = snack.IdCine,
                            Cantidad = snack.Cantidad,
                            Fecha = DateTime.Now
                        };
                        _context.EspectadorConsumibles.Add(espectadorConsumible);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["CompraExitosa"] = "Compra realizada con éxito.";
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al guardar la compra: {ex.Message}");
            return StatusCode(500, "Error interno al guardar la compra.");
        }
    }
    [HttpPost]
    public IActionResult VolverPago()
    {
        // No tocar PagoData → solo mantener lo que ya estaba guardado
        TempData.Keep("PagoData");

        TempData.Keep("IdProyeccion");
        TempData.Keep("CantidadEntradas");
        TempData.Keep("TotalCompra");
        TempData.Keep("FormatoEntrada");
        TempData.Keep("Asientos");

        return RedirectToAction("Index", "Pago");
    }


}
