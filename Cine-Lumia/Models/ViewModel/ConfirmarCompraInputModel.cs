using Cine_Lumia.Models;
using System.Collections.Generic;

namespace Cine_Lumia.Models.ViewModel
{
    public class ConfirmarCompraInputModel
    {
        public int IdProyeccion { get; set; }
        public List<string> AsientosSeleccionadosNombres { get; set; } = new List<string>();
        public string FormatoEntrada { get; set; } = string.Empty;
        public List<SnackSeleccionadoViewModel> SnacksSeleccionados { get; set; } = new List<SnackSeleccionadoViewModel>();
        public string MetodoPago { get; set; } = string.Empty;
    }
}
