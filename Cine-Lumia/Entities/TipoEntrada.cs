using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class TipoEntrada
    {
        [Key]
        public int Id_TipoEntrada { get; set; }

        [ForeignKey("Formato")]
        public int Id_Formato { get; set; }
        public Formato Formato { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        // Relación con entradas
        public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
    }
}
