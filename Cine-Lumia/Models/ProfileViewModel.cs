
using System.ComponentModel.DataAnnotations;

namespace Cine_Lumia.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string? Alias { get; set; }
        public int IdAvatar { get; set; }
        public List<string> Avatars { get; set; } = new List<string>();
    }
}
