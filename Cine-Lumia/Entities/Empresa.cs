using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Empresa
    {
        [Key]
        public int Id_Empresa { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Cine> Cines { get; set; } = new HashSet<Cine>();
    }
}
