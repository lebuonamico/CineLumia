using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class PeliculaGenero
    {
        [ForeignKey("Pelicula")]
        public int Id_Pelicula { get; set; }
        public Pelicula Pelicula { get; set; } = null!;

        [ForeignKey("Genero")]
        public int Id_Genero { get; set; }
        public Genero Genero { get; set; } = null!;
    }
}
