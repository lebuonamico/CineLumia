using System.Collections.Generic;

namespace Cine_Lumia.Models.ViewModel
{
    public class ComprasRealizadasViewModel
    {
        public List<TicketPurchaseViewModel> Tickets { get; set; } = new List<TicketPurchaseViewModel>();
        public List<SnackPurchaseViewModel> Snacks { get; set; } = new List<SnackPurchaseViewModel>();
    }

    public class TicketPurchaseViewModel
    {
        public int EntradaId { get; set; }
        public string PeliculaNombre { get; set; }
        public string Horario { get; set; }
        public string Asiento { get; set; }
        public string CineNombre { get; set; }
        public string CineDireccion { get; set; }
        public string ImagenUrl { get; set; } // Movie poster
        public DateTime FechaCompra { get; set; }
        public DateTime FechaHoraProyeccion { get; set; }
    }

    public class SnackPurchaseViewModel
    {
        public string ConsumibleNombre { get; set; }
        public int Cantidad { get; set; }
        // Optionally, if snacks have images, you could add:
        // public string ImagenUrl { get; set; }
    }
}
