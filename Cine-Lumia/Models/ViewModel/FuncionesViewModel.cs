using Cine_Lumia.Entities;

namespace Cine_Lumia.Models.ViewModel
{
    public class FuncionesViewModel
    {
        public Pelicula Pelicula { get; set; } = null!;
        public Cine Cine { get; set; } = null!;

        public string FechaSeleccionada { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
        public List<string> FechasDisponibles { get; set; } = new();

        // Diccionario: fecha "yyyy-MM-dd" → lista de salas con horarios
        public Dictionary<string, List<SalaConHorarios>> ProyeccionesPorFecha { get; set; } = new();
        public int Disponibles { get; set; }
    }

    public class SalaConHorarios
    {
        public string Sala { get; set; } = null!;
        public List<HorarioProyeccion> Horarios { get; set; } = new();
    }

    public class HorarioProyeccion
    {
        public string Hora { get; set; } = null!;
        public int IdProyeccion { get; set; }

        public int Disponibles { get; set; }      // cantidad de asientos disponibles
        public int TotalAsientos { get; set; }    // total de asientos en la sala
    }
}
