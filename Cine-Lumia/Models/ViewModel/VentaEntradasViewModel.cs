using Cine_Lumia.Entities;

namespace Cine_Lumia.Models.ViewModel
{
    public class VentaEntradasViewModel
    {
        public Proyeccion Proyeccion { get; set; } = null!;

        // Lista de tipos de entrada disponibles según formato de la sala
        public List<TipoEntrada> TiposEntrada { get; set; } = new();

        // Para acumular cantidades seleccionadas por el usuario
        public Dictionary<int, int> CantidadesSeleccionadas { get; set; } = new();

        // Total calculado (se puede actualizar en JS)
        public decimal Total { get; set; }
        public int TotalAsientos { get; set; }
        public int AsientosDisponibles { get; set; }
    }
}
