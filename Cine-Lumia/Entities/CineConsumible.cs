using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class CineConsumible
    {
        [ForeignKey("Cine")]
        public int Id_Cine { get; set; }
        public Cine Cine { get; set; } = null!;

        [ForeignKey("Consumible")]
        public int Id_Consumible { get; set; }
        public Consumible Consumible { get; set; } = null!;

        public int Stock { get; set; }
    }
}
