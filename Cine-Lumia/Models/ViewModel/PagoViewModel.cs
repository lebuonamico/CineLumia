using Cine_Lumia.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models.ViewModel
{
    public class PagoViewModel
    {
        // Datos de la proyección y asientos
        public Proyeccion Proyeccion { get; set; } = null!;
        public List<Asiento> AsientosSeleccionados { get; set; } = new();
        public int CantidadEntradas { get; set; }
        public string FormatoEntrada { get; set; } = null!;
        public decimal TotalCompra { get; set; }

        // Para facilitar la vista
        public int IdProyeccion => Proyeccion?.Id_Proyeccion ?? 0;
        public string Asientos => string.Join(",", AsientosSeleccionados.Select(a => a.Id_Asiento));

        // Datos del titular
        [Required(ErrorMessage = "El titular es obligatorio.")]
        public string Titular { get; set; } = "";

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,10}$", ErrorMessage = "Ingresá un DNI válido (solo números, 7 a 10 dígitos).")]
        public string DNI { get; set; } = "";


        // Datos de la tarjeta
        [Required(ErrorMessage = "El número de tarjeta es obligatorio.")]
        [CreditCard(ErrorMessage = "Ingresá un número de tarjeta válido.")]
        public string NumeroTarjeta { get; set; } = "";

        [Required(ErrorMessage = "El vencimiento es obligatorio.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Ingresá vencimiento válido (MM/YY).")]
        public string Vencimiento { get; set; } = "";

        [Required(ErrorMessage = "El CVV es obligatorio.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Ingresá un CVV válido.")]
        public string CVV { get; set; } = "";
    }
}
