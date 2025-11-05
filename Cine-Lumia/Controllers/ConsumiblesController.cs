using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cine_Lumia.Controllers
{
    public class ConsumiblesController : Controller
    {
        private readonly CineDbContext _context;

        public ConsumiblesController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var consumiblesPorCine = _context.CineConsumibles
                .Include(cc => cc.Cine)
                .Include(cc => cc.Consumible)
                .ToList();

            return View(consumiblesPorCine);
        }
    }
}