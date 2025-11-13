using Cine_Lumia.Entities;

namespace Cine_Lumia.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<Pelicula> Destacadas { get; set; } = new List<Pelicula>();
        public List<Pelicula> Cartelera { get; set; } = new List<Pelicula>();

        // Rutas relativas a wwwroot (ej: "/images/banner/archivo.webp")
        public List<string> BannerUrls { get; set; } = new List<string>();
    }
}
