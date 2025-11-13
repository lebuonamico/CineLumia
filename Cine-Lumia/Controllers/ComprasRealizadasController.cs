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

            var entradas = await _context.Entradas
                .Where(e => e.Id_Espectador == user.Id_Espectador)
                .Include(e => e.Proyeccion)
                    .ThenInclude(p => p.Pelicula)
                .Include(e => e.Proyeccion)
                    .ThenInclude(p => p.Sala)
                .Include(e => e.Asiento)
                .Select(e => new ComprasRealizadasViewModel
                {
                    PeliculaNombre = e.Proyeccion.Pelicula.Nombre,
                    Asiento = e.Asiento.Fila + e.Asiento.Columna.ToString(),
                    Horario = e.Proyeccion.Hora.ToString(@"hh\:mm"),
                    ImagenUrl = e.Proyeccion.Pelicula.PosterUrl,
                    Descripcion = e.Proyeccion.Pelicula.Nombre // Or some other description property
                })
                .ToListAsync();

            return View(entradas);
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