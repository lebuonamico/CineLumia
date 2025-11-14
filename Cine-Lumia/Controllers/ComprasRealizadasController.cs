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

            // Fetch movie ticket purchases
            var entradas = await _context.Entradas
                .Where(e => e.Id_Espectador == user.Id_Espectador)
                .Include(e => e.Proyeccion)
                    .ThenInclude(p => p.Pelicula)
                .Include(e => e.Proyeccion)
                    .ThenInclude(p => p.Sala)
                        .ThenInclude(s => s.Cine) // Include Cine for address
                .Include(e => e.Asiento)
                .ToListAsync();

            viewModel.Tickets = entradas.Select(e => new TicketPurchaseViewModel
            {
                PeliculaNombre = e.Proyeccion.Pelicula.Nombre,
                Horario = e.Proyeccion.Hora.ToString(@"hh\:mm"),
                Asiento = e.Asiento.Fila + e.Asiento.Columna.ToString(),
                CineNombre = e.Proyeccion.Sala.Cine.Nombre,
                CineDireccion = e.Proyeccion.Sala.Cine.Direccion,
                ImagenUrl = e.Proyeccion.Pelicula.PosterUrl
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