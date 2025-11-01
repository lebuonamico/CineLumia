using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Genero
    {
        [Key]
        public int Id_Genero { get; set; }

        [Required, StringLength(100)]
        public string GeneroNombre { get; set; } = string.Empty;

        // Relaciones
        public ICollection<PeliculaGenero> PeliculaGeneros { get; set; } = new HashSet<PeliculaGenero>();
    }
}
