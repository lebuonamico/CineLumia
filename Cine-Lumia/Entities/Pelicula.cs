using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Pelicula
    {
        [Key]
        public int Id_Pelicula { get; set; }

        public int Duracion { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Fecha_Estreno { get; set; }
        public string PosterUrl { get; set; } = "";

        // Relaciones
        public ICollection<PeliculaGenero> PeliculaGeneros { get; set; } = new HashSet<PeliculaGenero>();
        public ICollection<Proyeccion> Proyecciones { get; set; } = new HashSet<Proyeccion>();
    }
}
