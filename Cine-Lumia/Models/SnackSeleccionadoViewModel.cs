namespace Cine_Lumia.Models
{
    public class SnackSeleccionadoViewModel
    {
        // Identificador del consumible (snack)
        public int IdConsumible { get; set; }

        // Nombre del snack (por ejemplo: pochoclos, gaseosa, etc.)
        public string Nombre { get; set; } = string.Empty;

        // Precio unitario del snack
        public decimal Precio { get; set; }

        // Cantidad seleccionada
        public int Cantidad { get; set; }

        // Cine donde se está comprando
        public int IdCine { get; set; }

        // Nombre del cine (solo para mostrar en el resumen)
        public string CineNombre { get; set; } = string.Empty;
    }
}