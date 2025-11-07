using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class Proyeccion
    {
        [Key]
        public int Id_Proyeccion { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan Hora { get; set; }

        [ForeignKey("Sala")]
        public int Id_Sala { get; set; }
        public Sala Sala { get; set; } = null!;

        [ForeignKey("Pelicula")]
        public int Id_Pelicula { get; set; }
        public Pelicula Pelicula { get; set; } = null!;

        // Relaciones
        public ICollection<Entrada> Entradas { get; set; } = new HashSet<Entrada>();
    }
}
