namespace Cine_Lumia.Models.ViewModel
{

    public class CarritoItemViewModel
    {
        public int Id { get; set; }
        public string Snack { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int CineId { get; set; }
        public string ImagenUrl { get; set; }
    }
}
