using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ComprasRealizadasController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return Challenge();
            }

            var viewModel = new ComprasRealizadasViewModel();

            var ticketData = await _context.Entradas
                .Where(e => e.Id_Espectador == user.Id_Espectador)
                .Select(e => new
                {
                    e.Id_Entrada,
                    e.FechaCompra,
                    PeliculaNombre = e.Proyeccion.Pelicula.Nombre,
                    Horario = e.Proyeccion.Hora,
                    FechaHoraProyeccion = e.Proyeccion.Fecha.Add(e.Proyeccion.Hora),
                    AsientoFila = e.Asiento.Fila,
                    AsientoColumna = e.Asiento.Columna,
                    CineNombre = e.Proyeccion.Sala.Cine.Nombre,
                    CineDireccion = e.Proyeccion.Sala.Cine.Direccion,
                    ImagenUrl = e.Proyeccion.Pelicula.PosterUrl
                })
                .ToListAsync();

            viewModel.Tickets = ticketData.Select(t => new TicketPurchaseViewModel
            {
                EntradaId = t.Id_Entrada,
                PeliculaNombre = t.PeliculaNombre,
                Horario = t.Horario.ToString(@"hh\:mm"),
                Asiento = t.AsientoFila.ToString() + t.AsientoColumna.ToString(),
                CineNombre = t.CineNombre,
                CineDireccion = t.CineDireccion,
                ImagenUrl = t.ImagenUrl,
                FechaCompra = t.FechaCompra,
                FechaHoraProyeccion = t.FechaHoraProyeccion
            }).ToList();

            // Fetch snack purchases
            var snacksComprados = await _context.EspectadorConsumibles
                .Where(ec => ec.Id_Espectador == user.Id_Espectador)
                .Include(ec => ec.Consumible)
                .ToListAsync();

            viewModel.Snacks = snacksComprados.Select(ec => new SnackPurchaseViewModel
            {
                ConsumibleNombre = ec.Consumible.Nombre,
                Cantidad = ec.Cantidad
            }).ToList();

            // Temporary code for demonstration if no snacks are found
            if (!viewModel.Snacks.Any())
            {
                viewModel.Snacks.Add(new SnackPurchaseViewModel
                {
                    ConsumibleNombre = "Combo Amigos (Ejemplo)",
                    Cantidad = 1
                });
            }

            return View(viewModel);
        }

        private Espectador GetCurrentUser()
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