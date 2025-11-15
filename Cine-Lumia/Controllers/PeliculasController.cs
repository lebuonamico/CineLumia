using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;  
using Cine_Lumia.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cine_Lumia.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly CineDbContext _context;

        public PeliculasController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Peliculas
                .Include(p => p.PeliculaGeneros)
                .ThenInclude(pg => pg.Genero)
                .ToListAsync();
             
            //var peliculas = _context.Peliculas.ToList();
            return View(peliculas);
        }

        [HttpPost]
        public IActionResult VerFunciones(int peliculaId, int? cineId)
        {
            int cineSeleccionadoId = -1;

            // Si se envía cineId en el POST lo usamos (viene desde localStorage inyectado en el form)
            if (cineId.HasValue)
            {
                cineSeleccionadoId = cineId.Value;
                // Guardamos también como CineSeleccionado en TempData y Session para futuras acciones
                TempData["CineSeleccionado"] = cineSeleccionadoId;
                HttpContext.Session.SetInt32("CineSeleccionado", cineSeleccionadoId);
            }
            else if (TempData.ContainsKey("CineSeleccionado"))
            {
                // Usamos Peek para no consumir la clave y mantener la selección para futuras acciones
                cineSeleccionadoId = Convert.ToInt32(TempData.Peek("CineSeleccionado"));
            }
            else
            {
                // Intentar recuperar desde Session si TempData no tiene el valor (por navegación previa)
                var fromSession = HttpContext.Session.GetInt32("CineSeleccionado");
                if (fromSession.HasValue)
                {
                    cineSeleccionadoId = fromSession.Value;
                    // restauramos TempData para que Funciones pueda usarlo
                    TempData["CineSeleccionado"] = cineSeleccionadoId;
                }
            }

            if (cineSeleccionadoId > 0)
            {
                // Guardamos en TempData lo que necesita FuncionesController
                TempData["CineId"] = cineSeleccionadoId;
                TempData["PeliculaId"] = peliculaId;

                // Asegurar que CineSeleccionado no se pierda en la siguiente request
                TempData.Keep("CineSeleccionado");

                // Redirigimos a Funciones/VolverDesdeVentaEntradas para que use TempData y redireccione internamente
                return RedirectToAction("VolverDesdeVentaEntradas", "Funciones");
            }

            // Si no hay selección válida
            TempData.Keep();
            TempData["ErrorSeleccionCine"] = "Debe seleccionar un cine para ver detalles.";
            return RedirectToAction("Index", "Home");
        }
    }
}