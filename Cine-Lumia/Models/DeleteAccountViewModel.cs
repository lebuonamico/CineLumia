using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    public class DeleteAccountViewModel
    {
        [Required(ErrorMessage = "La contraseña es obligatoria para eliminar la cuenta.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
