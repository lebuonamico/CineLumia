using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    /// <summary>
    /// ViewModel utilizado para manejar los datos del formulario de inicio de sesión.
    /// Contiene las propiedades para el correo electrónico y la contraseña del usuario.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Propiedad para el correo electrónico del usuario.
        /// Es obligatorio y debe tener un formato de correo electrónico válido.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Email { get; set; }

        /// <summary>
        /// Propiedad para la contraseña del usuario.
        /// Es obligatoria y se muestra como tipo Password en la interfaz de usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
