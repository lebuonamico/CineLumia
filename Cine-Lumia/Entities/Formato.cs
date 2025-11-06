using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Formato
    {
        [Key]
        public int Id_Formato { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // Relación con salas y tipos de entrada
        public ICollection<Sala> Salas { get; set; } = new List<Sala>();
        public ICollection<TipoEntrada> TipoEntradas { get; set; } = new List<TipoEntrada>();
    }
}
