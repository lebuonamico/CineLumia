using Cine_Lumia.Entities;

namespace Cine_Lumia.Models.ViewModel
{
    public class ResumenCompraViewModel
    {
        public Proyeccion Proyeccion { get; set; } = null!;
        public List<Asiento> AsientosSeleccionados { get; set; } = new();
        public int CantidadEntradas { get; set; }
        public string FormatoEntrada { get; set; } = null!;
        public decimal TotalCompra { get; set; }
        public decimal CargoServicioEntradas { get; set; } = 1600m; // valor fijo, ajustable
        public string? MetodoPago { get; set; }

        public List<SnackSeleccionadoViewModel> SnacksSeleccionados { get; set; } = new List<SnackSeleccionadoViewModel>();
    }
}
