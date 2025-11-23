using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cine_Lumia.Controllers
{
    [Authorize]
    public class ComprasRealizadasController : Controller
    {
        private readonly CineDbContext _context;
        private const int PageSize = 5; // Define un tamaño de página


        public ComprasRealizadasController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int ticketsPage = 1, int snacksPage = 1)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return Challenge();
            }

            var viewModel = new ComprasRealizadasViewModel();

            // Entradas
            var allUserTickets = await _context.Entradas
                .Where(e => e.Id_Espectador == user.Id_Espectador)
                .Include(e => e.Proyeccion.Pelicula)
                .Include(e => e.Proyeccion.Sala.Cine)
                .Include(e => e.Asiento)
                .Include(e => e.TipoEntrada) // Incluir TipoEntrada para obtener el precio
                .OrderByDescending(e => e.FechaCompra)
                .ToListAsync();

            var groupedTickets = allUserTickets.GroupBy(e => e.Id_Proyeccion)
                .Select(g =>
                {
                    var firstTicket = g.First();
                    var totalEntradas = g.Sum(t => t.TipoEntrada.Precio); // Sumar el precio de cada entrada
                    return new TicketPurchaseViewModel
                    {
                        EntradasIds = g.Select(t => t.Id_Entrada).ToList(),
                        PeliculaNombre = firstTicket.Proyeccion.Pelicula.Nombre,
                        PeliculaAnio = firstTicket.Proyeccion.Pelicula.Fecha_Estreno.Year,
                        PeliculaDuracion = firstTicket.Proyeccion.Pelicula.Duracion,
                        Horario = firstTicket.Proyeccion.Hora.ToString(@"hh\:mm"),
                        FechaHoraProyeccion = firstTicket.Proyeccion.Fecha.Add(firstTicket.Proyeccion.Hora),
                        CineNombre = firstTicket.Proyeccion.Sala.Cine.Nombre,
                        CineDireccion = firstTicket.Proyeccion.Sala.Cine.Direccion,
                        ImagenUrl = firstTicket.Proyeccion.Pelicula.PosterUrl,
                        FechaCompra = firstTicket.FechaCompra,
                        Cantidad = g.Count(),
                        Asientos = g.Select(t => $"{t.Asiento.Fila}{t.Asiento.Columna}").ToList(),
                        TotalEntradas = totalEntradas
                    };
                }).ToList();

            // Snacks
            var allUserSnacks = await _context.EspectadorConsumibles
                .Where(ec => ec.Id_Espectador == user.Id_Espectador)
                .Include(ec => ec.Consumible)
                .OrderByDescending(ec => ec.Fecha)
                .Select(ec => new SnackPurchaseViewModel
                {
                    ConsumibleNombre = ec.Consumible.Nombre,
                    Cantidad = ec.Cantidad,
                    FechaCompra = ec.Fecha,
                    Precio = ec.Consumible.Precio,
                    ImagenUrl = ec.Consumible.PosterUrl
                })
                .ToListAsync();

            var snacksSinEntrada = new List<SnackPurchaseViewModel>(allUserSnacks);

            const decimal TicketSurcharge = 1600m;
            const decimal SnackSurcharge = 700m;

            foreach (var ticketGroup in groupedTickets)
            {
                var snacksAsociados = snacksSinEntrada
                    .Where(snack => Math.Abs((snack.FechaCompra - ticketGroup.FechaCompra).TotalMinutes) < 2)
                    .ToList();

                if (snacksAsociados.Any())
                {
                    ticketGroup.ConsumiblesAsociados.AddRange(snacksAsociados);
                    snacksSinEntrada.RemoveAll(snack => snacksAsociados.Contains(snack));
                }

                // Calcular explícitamente los totales
                ticketGroup.TotalSnacks = ticketGroup.ConsumiblesAsociados.Sum(s => s.Precio * s.Cantidad);

                decimal total = ticketGroup.TotalEntradas + ticketGroup.TotalSnacks;
                if (ticketGroup.TotalEntradas > 0)
                {
                    total += TicketSurcharge;
                }
                if (ticketGroup.TotalSnacks > 0)
                {
                    total += SnackSurcharge;
                }
                ticketGroup.TotalCompra = total;
            }

            // Paginación para Entradas
            viewModel.TicketsTotalPages = (int)Math.Ceiling(groupedTickets.Count / (double)PageSize);
            viewModel.Tickets = groupedTickets.Skip((ticketsPage - 1) * PageSize).Take(PageSize).ToList();
            viewModel.TicketsPage = ticketsPage;

            // Agrupar los snacks sin entrada en combos
            var snackCombos = snacksSinEntrada
                .GroupBy(s => new DateTime(s.FechaCompra.Year, s.FechaCompra.Month, s.FechaCompra.Day, s.FechaCompra.Hour, s.FechaCompra.Minute, s.FechaCompra.Second))
                .Select(g => new SnackComboViewModel
                {
                    FechaCompra = g.Key,
                    PrecioTotal = g.Sum(s => s.Cantidad * s.Precio),
                    CantidadTotalItems = g.Sum(s => s.Cantidad),
                    DescripcionItems = string.Join(" + ", g
                        .GroupBy(s => s.ConsumibleNombre)
                        .Select(itemGroup => $"{itemGroup.Sum(i => i.Cantidad)}x {itemGroup.Key}")
                        .Take(5))
                        + (g.GroupBy(s => s.ConsumibleNombre).Count() > 5 ? "..." : "")
                })
                .ToList();


            // Paginación para Snacks
            viewModel.SnacksTotalPages = (int)Math.Ceiling(snackCombos.Count / (double)PageSize);
            viewModel.Snacks = snackCombos.Skip((snacksPage - 1) * PageSize).Take(PageSize).ToList();
            viewModel.SnacksPage = snacksPage;

            return View(viewModel);
        }




        private Espectador? GetCurrentUser()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return null;
            }
            return _context.Espectadores.FirstOrDefault(e => e.Email == userEmail);
        }
    }
}