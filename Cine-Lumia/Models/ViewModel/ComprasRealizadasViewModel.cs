using System;
using System.Collections.Generic;
using System.Linq;

namespace Cine_Lumia.Models.ViewModel
{
    public class ComprasRealizadasViewModel
    {
        public List<TicketPurchaseViewModel> Tickets { get; set; } = new List<TicketPurchaseViewModel>();
        public List<SnackComboViewModel> Snacks { get; set; } = new List<SnackComboViewModel>();
        public int TicketsPage { get; set; }
        public int TicketsTotalPages { get; set; }
        public int SnacksPage { get; set; }
        public int SnacksTotalPages { get; set; }
    }

    public class SnackComboViewModel
    {
        public string ImagenUrl { get; set; } = "/images/snacks/000-COMBO.jpg";
        public decimal PrecioTotal { get; set; }
        public int CantidadTotalItems { get; set; }
        public string DescripcionItems { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
    }

    public class TicketPurchaseViewModel
    {
        public List<int> EntradasIds { get; set; } = new List<int>();
        public string? PeliculaNombre { get; set; }
        public int PeliculaAnio { get; set; }
        public int PeliculaDuracion { get; set; }
        public string? Horario { get; set; }
        public List<string> Asientos { get; set; } = new List<string>();
        public int Cantidad { get; set; }
        public string? CineNombre { get; set; }
        public string? CineDireccion { get; set; }
        public string? ImagenUrl { get; set; } // Movie poster
        public DateTime FechaCompra { get; set; }
        public DateTime FechaHoraProyeccion { get; set; }
        public List<SnackPurchaseViewModel> ConsumiblesAsociados { get; set; } = new List<SnackPurchaseViewModel>();
        public decimal TotalEntradas { get; set; }
        public decimal TotalSnacks { get; set; }
        public decimal TotalCompra { get; set; }
    }

    public class SnackPurchaseViewModel
    {
        public string? ConsumibleNombre { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
