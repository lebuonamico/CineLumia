
using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    public class ManageViewModel
    {
        public string Email { get; set; }

        public string? Dni { get; set; }

        public string? Alias { get; set; }

        [Required(ErrorMessage = "La contraseña actual es obligatoria para guardar los cambios.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; }
    }
}
