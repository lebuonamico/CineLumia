using Cine_Lumia.Entities;

namespace Cine_Lumia.Models
{
    public class DashboardViewModel
    {
        public List<Empresa> Empresas { get; set; }
        public List<Pelicula> Peliculas { get; set; }
        public List<CineConsumible> CineConsumibles { get; set; }
        public List<EspectadorConsumible> Compras { get; set; }
    }
}
