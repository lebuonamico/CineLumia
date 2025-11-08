using Cine_Lumia.Entities;

namespace Cine_Lumia.Models
{
    public class DashboardViewModel
    {
        public List<Empresa> Empresas { get; set; } = new List<Empresa>();
        public List<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
        public List<CineConsumible> CineConsumibles { get; set; } = new List<CineConsumible>();
        public List<EspectadorConsumible> Compras { get; set; } = new List<EspectadorConsumible>();
    }
}
