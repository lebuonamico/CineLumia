using Cine_Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Cine_Lumia.Entities;

namespace Cine_Lumia.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión de cuentas de usuario, incluyendo el inicio de sesión.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly CineDbContext _context;

        /// <summary>
        /// Constructor del controlador que inyecta el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Contexto de la base de datos de Cine Lumia.</param>
        public AccountController(CineDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra la vista del formulario de inicio de sesión.
        /// </summary>
        /// <returns>Vista del formulario de login.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario de inicio de sesión.
        /// Valida las credenciales del usuario contra la base de datos.
        /// </summary>
        /// <param name="model">Modelo con el email y la contraseña del usuario.</param>
        /// <returns>Redirecciona a la página principal si el login es exitoso, de lo contrario, vuelve a mostrar el formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Busca un espectador que coincida con el email y la contraseña proporcionados.
                // NOTA: En una aplicación real, la contraseña debería estar hasheada y verificada de forma segura.
                var espectador = _context.Espectadores.FirstOrDefault(e => e.Email == model.Email && e.Password == model.Password);

                if (espectador != null)
                {
                    // Si el espectador existe, redirige a la página de inicio.
                    // En una aplicación real, aquí se establecería una sesión de usuario (ej. HttpContext.SignInAsync).
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si no se encuentra el usuario, añade un error al ModelState.
                    ModelState.AddModelError(string.Empty, "El usuario no existe o la contraseña es incorrecta.");
                }
            }
            // Si el modelo no es válido o el login falla, vuelve a mostrar la vista con los errores.
            return View(model);
        }
    }
}
