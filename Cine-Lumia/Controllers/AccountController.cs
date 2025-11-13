using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
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
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Pass returnUrl back to view if model state is invalid
            if (ModelState.IsValid)
            {
                // Busca un espectador que coincida con el email o alias y la contraseña proporcionados.
                // NOTA: En una aplicación real, la contraseña debería estar hasheada y verificada de forma segura.
                var espectador = _context.Espectadores.FirstOrDefault(e =>
                    (e.Email == model.LoginIdentifier || e.Alias == model.LoginIdentifier) &&
                    e.Password == model.Password);

                if (espectador != null)
                {
                    // TODO: Si se implementa verificación por email, añadir aquí la comprobación de cuenta verificada.
                    // if (!espectador.IsVerified) {
                    //     ModelState.AddModelError(string.Empty, "Cuenta no verificada, revisá tu correo.");
                    //     return View(model);
                    // }

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, espectador.Email),
                        new Claim(ClaimTypes.Email, espectador.Email),
                        new Claim("Alias", espectador.Alias ?? espectador.Email) // Add alias claim
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "LumiaCookieAuth");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // Usa el valor del checkbox "Recordarme"
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1) // 30 días si "Recordarme", 1 día si no
                    };

                    await HttpContext.SignInAsync("LumiaCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Redirect to home if no valid returnUrl
                    }
                }
                else
                {
                    // Si no se encuentra el usuario, añade un error al ModelState.
                    ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                }
            }
            // Si el modelo no es válido o el login falla, vuelve a mostrar la vista con los errores.
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                Email = user.Email,
                Alias = user.Alias,
                IdAvatar = user.IdAvatar,
                Avatars = GetAvatarList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Espectadores.FirstOrDefault(e => e.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
                    model.Email = string.Empty; // Clear email field
                    model.Password = string.Empty; // Clear password field
                    model.ConfirmPassword = string.Empty; // Clear confirm password field
                    return View(model);
                }

                var espectador = new Espectador
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Email = model.Email,
                    Password = model.Password, // En una aplicación real, hashear la contraseña
                    Dni = null // DNI es requerido, lo pongo en null por ahora.
                };

                _context.Espectadores.Add(espectador);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para enviar el correo de recuperación.
                // Por ahora, solo redirigimos a una página de confirmación.
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Cierra la sesión y elimina la cookie de autenticación
            await HttpContext.SignOutAsync("LumiaCookieAuth");

            // 🔒 Evita que el navegador almacene en caché las páginas protegidas
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            // Redirige al Home (o podrías redirigir al Login si preferís)
            return RedirectToAction("Index", "Home");
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

        private List<string> GetAvatarList()
        {
            // This could be read from a configuration file or a directory listing in a real application
            return new List<string> { "AV001.gif", "AV002.gif", "AV003.gif" };
        }

        [HttpGet]
        [Authorize]
        public IActionResult Manage()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(user.Alias))
            {
                var random = new Random();
                user.Alias = $"CLIENTE_{random.Next(100000, 999999)}";
                _context.SaveChanges();
            }

            var model = new ManageViewModel
            {
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                Dni = user.Dni,
                Alias = user.Alias,
                Telefono = user.Telefono,
                FechaNacimiento = user.FechaNacimiento,
                Genero = user.Genero,
                IdAvatar = user.IdAvatar,
                Avatars = GetAvatarList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Manage(ManageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate avatars list if model state is invalid
                model.Avatars = GetAvatarList();
                return View(model);
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }



            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
                model.Avatars = GetAvatarList(); // Repopulate avatars list if model state is invalid
                return View(model);
            }

            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.Email = model.Email;
            user.Dni = model.Dni;
            user.Alias = model.Alias;
            user.Telefono = model.Telefono;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Genero = model.Genero;
            user.IdAvatar = model.IdAvatar;

            _context.SaveChanges();

            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "La contraseña actual es incorrecta.");
                return View(model);
            }

            user.Password = model.NewPassword; // En una aplicación real, hashear la contraseña
            _context.SaveChanges();

            return RedirectToAction("Manage");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteAccountConfirmed(DeleteAccountPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model is invalid, return a JSON response with errors for AJAX handling or reload the page with error
                // For now, we'll just redirect to manage, an actual implementation might use AJAX and return Json(errors)
                return RedirectToAction("Manage");
            }

            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // In a real application, you would use ASP.NET Core Identity's password hasher
            // For now, simple string comparison
            if (user.Password != model.Password)
            {
                ModelState.AddModelError("Password", "La contraseña es incorrecta.");
                // Again, for AJAX this would be Json(errors), for full post it would reload manage view
                return RedirectToAction("Manage");
            }

            _context.Espectadores.Remove(user);
            await HttpContext.SignOutAsync("LumiaCookieAuth"); // Sign out the user BEFORE saving changes to avoid issues
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect to home page
        }

        // Internal class for password confirmation in DeleteAccountConfirmed action
        public class DeleteAccountPasswordModel
        {
            [Required(ErrorMessage = "La contraseña es obligatoria para eliminar la cuenta.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}