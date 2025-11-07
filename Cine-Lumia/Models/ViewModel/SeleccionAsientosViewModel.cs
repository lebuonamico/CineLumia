using Cine_Lumia.Entities;

namespace Cine_Lumia.Models.ViewModel
{
    public class SeleccionAsientoViewModel
    {
        public int Id_Proyeccion { get; set; }
        public Proyeccion Proyeccion { get; set; } = null!;
        public List<Asiento> Asientos { get; set; } = new();
        public int Columnas { get; set; }
        public int Filas { get; set; }
        public int CantidadEntradas { get; set; }

        // Datos que vienen de VentaEntradas
        public string FormatoEntrada { get; set; } = null!;
        public decimal TotalCompra { get; set; }
    }
}
