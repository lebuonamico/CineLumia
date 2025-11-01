using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class Cine
    {
        [Key]
        public int Id_Cine { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Direccion { get; set; } = string.Empty;

        [ForeignKey("Empresa")]
        public int Id_Empresa { get; set; }
        public Empresa Empresa { get; set; } = null!;

        // Relaciones
        public ICollection<Sala> Salas { get; set; } = new HashSet<Sala>();
        public ICollection<CineConsumible> CineConsumibles { get; set; } = new HashSet<CineConsumible>();
        public ICollection<EspectadorConsumible> EspectadorConsumibles { get; set; } = new HashSet<EspectadorConsumible>();
    }
}
