namespace Cine_Lumia.Models.ViewModel
{
    public class SeleccionAsientosViewModel
    {
        public int IdProyeccion { get; set; }
        public string SalaNombre { get; set; } = "";
        public List<AsientoVM> Asientos { get; set; } = new();
    }

    public class AsientoVM
    {
        public int IdAsiento { get; set; }
        public string Fila { get; set; } = "";
        public int Columna { get; set; }
        public bool Ocupado { get; set; }
    }
}
