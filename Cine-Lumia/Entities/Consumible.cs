using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Consumible
    {
        [Key]
        public int Id_Consumible { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        [Required, Range(0, 999999)]
        [Precision(10, 2)]
        public decimal Precio { get; set; }

        // Relaciones
        public ICollection<CineConsumible> CineConsumibles { get; set; } = new HashSet<CineConsumible>();
        public ICollection<EspectadorConsumible> EspectadorConsumibles { get; set; } = new HashSet<EspectadorConsumible>();
    }
}
