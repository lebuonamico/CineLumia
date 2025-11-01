using Microsoft.AspNetCore.Mvc;

namespace Cine_Lumia.Controllers
{
    public class MockBDController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
