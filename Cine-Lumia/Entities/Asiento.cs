using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class Asiento
    {
        [Key]
        public int Id_Asiento { get; set; }

        [Required]
        [StringLength(1)]
        public string Fila { get; set; } = string.Empty;

        [Required]
        public int Columna { get; set; }

        [Required]
        public bool Disponible { get; set; }

        [ForeignKey("Sala")]
        public int Id_Sala { get; set; }
        public Sala Sala { get; set; } = null!;

        // Relaciones
        public ICollection<Entrada> Entradas { get; set; } = new HashSet<Entrada>();
    }
}
