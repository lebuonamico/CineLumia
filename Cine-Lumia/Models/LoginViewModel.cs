using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    /// <summary>
    /// ViewModel utilizado para manejar los datos del formulario de inicio de sesión.
    /// Contiene las propiedades para el correo electrónico o alias y la contraseña del usuario.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Propiedad para el correo electrónico o alias del usuario.
        /// Es obligatorio.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico o alias es obligatorio.")]
        [Display(Name = "Correo electrónico o Alias")]
        public string LoginIdentifier { get; set; }

        /// <summary>
        /// Propiedad para la contraseña del usuario.
        /// Es obligatoria y se muestra como tipo Password en la interfaz de usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Propiedad para la opción "Recordarme".
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
