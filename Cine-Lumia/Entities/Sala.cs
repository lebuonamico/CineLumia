using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class Sala
    {
        [Key]
        public int Id_Sala { get; set; }

        public int Cant_Butacas { get; set; }
        public int Cant_Filas { get; set; }
        public int Capacidad { get; set; }

        [ForeignKey("Cine")]
        public int Id_Cine { get; set; }
        public Cine Cine { get; set; } = null!;
        public string Formato { get; set; } = "";

        // Relaciones
        public ICollection<Asiento> Asientos { get; set; } = new HashSet<Asiento>();
        public ICollection<Proyeccion> Proyecciones { get; set; } = new HashSet<Proyeccion>();
    }
}
