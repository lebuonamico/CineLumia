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

            // 1. Fetch all tickets and snacks for the user
            var allUserTickets = await _context.Entradas
                .Where(e => e.Id_Espectador == user.Id_Espectador)
                .Include(e => e.Proyeccion.Pelicula)
                .Include(e => e.Proyeccion.Sala.Cine)
                .Include(e => e.Asiento)
                .Include(e => e.TipoEntrada)
                .OrderByDescending(e => e.FechaCompra)
                .ToListAsync();

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

            // 2. Group tickets by purchase time (down to the second)
            var ticketPurchases = allUserTickets
                .GroupBy(e => e.FechaCompra.ToString("yyyy-MM-dd HH:mm:ss"))
                .Select(g =>
                {
                    var firstTicket = g.First();
                    var totalEntradas = g.Sum(t => t.TipoEntrada.Precio);

                    // Find associated snacks
                    var associatedSnacks = allUserSnacks
                        .Where(s => s.FechaCompra.ToString("yyyy-MM-dd HH:mm:ss") == g.Key)
                        .ToList();

                    var totalSnacks = associatedSnacks.Sum(s => s.Precio * s.Cantidad);

                    // Calculate final total with surcharges
                    const decimal TicketSurcharge = 1600m;
                    const decimal SnackSurcharge = 700m;
                    decimal total = totalEntradas + totalSnacks;
                    if (totalEntradas > 0)
                    {
                        total += TicketSurcharge;
                    }
                    if (totalSnacks > 0)
                    {
                        total += SnackSurcharge;
                    }

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
                        TotalEntradas = totalEntradas,
                        ConsumiblesAsociados = associatedSnacks,
                        TotalSnacks = totalSnacks,
                        TotalCompra = total
                    };
                })
                .ToList();

            // 3. Identify snacks that were NOT associated with any ticket purchase
            var associatedSnacksSet = new HashSet<SnackPurchaseViewModel>(ticketPurchases.SelectMany(p => p.ConsumiblesAsociados));
            var snacksSinEntrada = allUserSnacks.Where(s => !associatedSnacksSet.Contains(s)).ToList();


            // Paginación para Entradas (Compras combinadas)
            viewModel.TicketsTotalPages = (int)Math.Ceiling(ticketPurchases.Count / (double)PageSize);
            viewModel.Tickets = ticketPurchases.Skip((ticketsPage - 1) * PageSize).Take(PageSize).ToList();
            viewModel.TicketsPage = ticketsPage;

            // Agrupar los snacks sin entrada en combos
            var snackCombos = snacksSinEntrada
                .GroupBy(s => s.FechaCompra.ToString("yyyy-MM-dd HH:mm:ss"))
                .Select(g => new SnackComboViewModel
                {
                    FechaCompra = DateTime.Parse(g.Key),
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