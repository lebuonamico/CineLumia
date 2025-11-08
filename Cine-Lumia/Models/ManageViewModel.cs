
using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    public class ManageViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "DNI")]
        public string? Dni { get; set; }

        [Display(Name = "Alias")]
        public string? Alias { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [Display(Name = "Teléfono de Contacto")]
        public string? Telefono { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Género")]
        public string? Genero { get; set; }

        public int IdAvatar { get; set; }

        public List<string> Avatars { get; set; } = new List<string>();

        [Required(ErrorMessage = "La contraseña actual es obligatoria para guardar los cambios.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
