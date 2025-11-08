using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Entities
{
    public class Espectador
    {
        [Key]
        public int Id_Espectador { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(50)]
        public string? Genero { get; set; }

        public string? Dni { get; set; }

        [StringLength(255)]
        public string? Alias { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Password { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Entrada> Entradas { get; set; } = new HashSet<Entrada>();
        public ICollection<EspectadorConsumible> EspectadorConsumibles { get; set; } = new HashSet<EspectadorConsumible>();
    }
}
