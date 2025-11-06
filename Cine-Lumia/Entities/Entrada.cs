using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Lumia.Entities
{
    public class Entrada
    {
        [Key]
        public int Id_Entrada { get; set; }

        [ForeignKey("Proyeccion")]
        public int Id_Proyeccion { get; set; }
        public Proyeccion Proyeccion { get; set; } = null!;

        [ForeignKey("Espectador")]
        public int Id_Espectador { get; set; }
        public Espectador Espectador { get; set; } = null!;

        [ForeignKey("Asiento")]
        public int Id_Asiento { get; set; }
        public Asiento Asiento { get; set; } = null!;

        // FK a TipoEntrada
        [ForeignKey("TipoEntrada")]
        public int Id_TipoEntrada { get; set; }
        public TipoEntrada TipoEntrada { get; set; } = null!;

        public DateTime FechaCompra { get; set; } = DateTime.Now;
    }
}
