using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class EspectadorConsumible
    {
        [Key]
        public int Id_Compra { get; set; }

        [ForeignKey("Espectador")]
        public int Id_Espectador { get; set; }
        public Espectador Espectador { get; set; } = null!;

        [ForeignKey("Consumible")]
        public int Id_Consumible { get; set; }
        public Consumible Consumible { get; set; } = null!;

        [ForeignKey("Cine")]
        public int Id_Cine { get; set; }
        public Cine Cine { get; set; } = null!;

        public int Cantidad { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
    }
}
