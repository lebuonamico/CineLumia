using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Data
{
    public static class CineSeeder
    {
        public static void Seed(CineDbContext context)
        {
            // =====================
            // Evitar duplicados
            // =====================
            if (context.Empresas.Any()) return;

            // =====================
            // EMPRESA
            // =====================
            var empresa = new Empresa { Nombre = "CINE LUMIA" };
            context.Empresas.Add(empresa);
            context.SaveChanges();

            // =====================
            // CINES
            // =====================
            var cine1 = new Cine { Nombre = "Lumia Centro", Direccion = "Av. Corrientes 1234", Empresa = empresa };
            var cine2 = new Cine { Nombre = "Lumia Norte", Direccion = "Panamericana 4500", Empresa = empresa };
            var cine3 = new Cine { Nombre = "Lumia Sur", Direccion = "Av. San Martín 8900", Empresa = empresa };
            context.Cines.AddRange(cine1, cine2, cine3);
            context.SaveChanges();
            // FORMATOS
            var formato2D = new Formato { Nombre = "2D" };
            var formato3D = new Formato { Nombre = "3D" };
            var formato4D = new Formato { Nombre = "4D" };
            var formatoXD = new Formato { Nombre = "XD" };
            context.Formato.AddRange(formato2D, formato3D, formato4D, formatoXD);
            context.SaveChanges();
            // =====================
            // SALAS con Formato
            // =====================
            var sala1 = new Sala { Cant_Butacas = 10, Cant_Filas = 10, Capacidad = 100, Cine = cine1, Id_Formato = formato2D.Id_Formato };
            var sala2 = new Sala { Cant_Butacas = 10, Cant_Filas = 8, Capacidad = 80, Cine = cine1, Id_Formato = formato3D.Id_Formato };
            var sala3 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine2, Id_Formato = formato2D.Id_Formato };
            var sala4 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine2, Id_Formato = formatoXD.Id_Formato };
            var sala5 = new Sala { Cant_Butacas = 8, Cant_Filas = 8, Capacidad = 64, Cine = cine2, Id_Formato = formato4D.Id_Formato };
            var sala6 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine3, Id_Formato = formato2D.Id_Formato };
            context.Salas.AddRange(sala1, sala2, sala3, sala4, sala5, sala6);
            context.SaveChanges();

            // =====================
            // ASIENTOS (solo algunos)
            // =====================
            var asientos = new List<Asiento>
            {
                new Asiento { Fila = "A", Columna = 1, Disponible = true, Sala = sala1 },
                new Asiento { Fila = "A", Columna = 2, Disponible = true, Sala = sala1 },
                new Asiento { Fila = "A", Columna = 3, Disponible = true, Sala = sala1 },
                new Asiento { Fila = "B", Columna = 1, Disponible = true, Sala = sala2 },
                new Asiento { Fila = "B", Columna = 2, Disponible = true, Sala = sala2 },
                new Asiento { Fila = "C", Columna = 1, Disponible = true, Sala = sala3 },
                new Asiento { Fila = "C", Columna = 2, Disponible = true, Sala = sala3 },
                new Asiento { Fila = "D", Columna = 1, Disponible = true, Sala = sala4 },
                new Asiento { Fila = "D", Columna = 2, Disponible = true, Sala = sala4 },
                new Asiento { Fila = "E", Columna = 1, Disponible = true, Sala = sala5 },
                new Asiento { Fila = "E", Columna = 2, Disponible = true, Sala = sala5 },
            };
            context.Asientos.AddRange(asientos);
            context.SaveChanges();

            // TIPOS DE ENTRADA (solo precio por formato)
            var tipo2D = new TipoEntrada { Id_Formato = formato2D.Id_Formato, Precio = 3500m };
            var tipo3D = new TipoEntrada { Id_Formato = formato3D.Id_Formato, Precio = 4800m };
            var tipo4D = new TipoEntrada { Id_Formato = formato4D.Id_Formato, Precio = 6000m };
            var tipoXD = new TipoEntrada { Id_Formato = formatoXD.Id_Formato, Precio = 5500m };
            context.TipoEntrada.AddRange(tipo2D, tipo3D, tipo4D, tipoXD);
            context.SaveChanges();

            // =====================
            // GENEROS
            // =====================
            var genero1 = new Genero { GeneroNombre = "Acción" };
            var genero2 = new Genero { GeneroNombre = "Comedia" };
            var genero3 = new Genero { GeneroNombre = "Terror" };
            var genero4 = new Genero { GeneroNombre = "Drama" };
            context.Generos.AddRange(genero1, genero2, genero3, genero4);
            context.SaveChanges();

            // =====================
            // PELICULAS
            // =====================
            var peliculas = new List<Pelicula>
            {
                new Pelicula { Nombre = "Avatar", Duracion = 162, PosterUrl = "/images/peliculas/avatar.jpg" },
                new Pelicula { Nombre = "Avengers: Endgame", Duracion = 181, PosterUrl = "/images/peliculas/avengers_endgame.jpg" },
                new Pelicula { Nombre = "The Dark Knight", Duracion = 152, PosterUrl = "/images/peliculas/batman_dark_knight.jpg" },
                new Pelicula { Nombre = "Dune", Duracion = 155, PosterUrl = "/images/peliculas/dune.jpg" },
                new Pelicula { Nombre = "Frozen", Duracion = 102, PosterUrl = "/images/peliculas/frozen.jpg" }
            };
            context.Peliculas.AddRange(peliculas);
            context.SaveChanges();

            // =====================
            // PELICULA_GENERO
            // =====================
            context.PeliculaGeneros.AddRange(
                new PeliculaGenero { Pelicula = peliculas[0], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[1], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[2], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[3], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[4], Genero = genero2 }
            );
            context.SaveChanges();

            // =====================
            // PROYECCIONES
            // =====================
            var horarios = new List<TimeSpan>
            {
                new TimeSpan(15, 45, 00),
                new TimeSpan(18, 15, 00),
                new TimeSpan(22, 30, 00)
            };

            var proyecciones = new List<Proyeccion>();

            // Para cada cine
            foreach (var cine in context.Cines.Include(c => c.Salas))
            {
                // Para cada película
                foreach (var peli in peliculas)
                {
                    // Para los próximos 7 días (hoy + 6)
                    for (int d = 0; d < 7; d++)
                    {
                        var fecha = DateTime.Today.AddDays(d);

                        // Para cada sala del cine
                        foreach (var sala in cine.Salas)
                        {
                            // 3 horarios por día
                            foreach (var h in horarios)
                            {
                                proyecciones.Add(new Proyeccion
                                {
                                    Pelicula = peli,
                                    Sala = sala,
                                    Fecha = fecha,
                                    Hora = fecha.Add(h)
                                });
                            }
                        }
                    }
                }
            }
            context.Proyecciones.AddRange(proyecciones);
            context.SaveChanges();

            // =====================
            // ESPECTADORES
            // =====================
            var espect1 = new Espectador { Dni = 40123123, Email = "espectador1@example.com", Password = "password123" };
            var espect2 = new Espectador { Dni = 39222444, Email = "espectador2@example.com", Password = "password123" };
            context.Espectadores.AddRange(espect1, espect2);
            context.SaveChanges();

            // =====================
            // ENTRADAS (ejemplo)
            // =====================
            // ENTRADAS de ejemplo
            context.Entradas.AddRange(
                new Entrada
                {
                    Proyeccion = proyecciones[0],
                    Espectador = espect1,
                    Asiento = asientos[0],
                    Id_TipoEntrada = tipo2D.Id_TipoEntrada,
                },
                new Entrada
                {
                    Proyeccion = proyecciones[1],
                    Espectador = espect2,
                    Asiento = asientos[4],
                    Id_TipoEntrada = tipo3D.Id_TipoEntrada,
                }
            );
            context.SaveChanges();

            // =====================
            // CONSUMIBLES
            // =====================
            var c1 = new Consumible { Nombre = "Pochoclos", Descripcion = "Pochoclos salados tamaño grande", Precio = 3500 };
            var c2 = new Consumible { Nombre = "Gaseosa", Descripcion = "Coca-Cola 500ml", Precio = 2500 };
            var c3 = new Consumible { Nombre = "Nachos", Descripcion = "Con queso cheddar", Precio = 3000 };
            context.Consumibles.AddRange(c1, c2, c3);
            context.SaveChanges();

            // =====================
            // CINE_CONSUMIBLE
            // =====================
            context.CineConsumibles.AddRange(
                new CineConsumible { Cine = cine1, Consumible = c1, Stock = 200 },
                new CineConsumible { Cine = cine2, Consumible = c2, Stock = 150 },
                new CineConsumible { Cine = cine3, Consumible = c3, Stock = 100 }
            );
            context.SaveChanges();
        }
    }
}
