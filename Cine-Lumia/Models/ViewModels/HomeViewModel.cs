using System.Collections.Generic;
using Cine_Lumia.Entities;

namespace Cine_Lumia.Models
{
 public class HomeViewModel
 {
 public List<Pelicula> Destacadas { get; set; } = new List<Pelicula>();
 public List<Pelicula> Cartelera { get; set; } = new List<Pelicula>();
 }
}
