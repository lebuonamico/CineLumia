using Cine_Lumia.Entities;
using Cine_Lumia.Models;

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

            // =====================
            // SALAS con Formato
            // =====================
            var sala1 = new Sala { Cant_Butacas = 10, Cant_Filas = 10, Capacidad = 100, Cine = cine1, Formato = "2D" };
            var sala2 = new Sala { Cant_Butacas = 10, Cant_Filas = 8, Capacidad = 80, Cine = cine1, Formato = "3D" };
            var sala3 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine2, Formato = "2D" };
            var sala4 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine2, Formato = "XD" };
            var sala5 = new Sala { Cant_Butacas = 8, Cant_Filas = 8, Capacidad = 64, Cine = cine2, Formato = "4D" };
            var sala6 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine3, Formato = "2D" };
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
    new Pelicula { Nombre = "Fast & Furious 7", Duracion = 137, PosterUrl = "/images/peliculas/fast_and_furious_7.jpg" },
    new Pelicula { Nombre = "Frozen", Duracion = 102, PosterUrl = "/images/peliculas/frozen.jpg" },
    new Pelicula { Nombre = "Harry Potter and the Sorcerer's Stone", Duracion = 152, PosterUrl = "/images/peliculas/harry_potter_stone.jpg" },
    new Pelicula { Nombre = "Inception", Duracion = 148, PosterUrl = "/images/peliculas/inception.jpg" },
    new Pelicula { Nombre = "Interstellar", Duracion = 169, PosterUrl = "/images/peliculas/interstellar.jpg" },
    new Pelicula { Nombre = "James Bond: Skyfall", Duracion = 143, PosterUrl = "/images/peliculas/james_bond_skyfall.jpg" },
    new Pelicula { Nombre = "Jurassic World", Duracion = 124, PosterUrl = "/images/peliculas/jurassic_world.jpg" },
    new Pelicula { Nombre = "Minecraft", Duracion = 110, PosterUrl = "/images/peliculas/minecraft.jpg" },
    new Pelicula { Nombre = "Spiderman: No Way Home", Duracion = 148, PosterUrl = "/images/peliculas/spiderman_no_way_home.jpg" },
    new Pelicula { Nombre = "The Super Mario Bros Movie", Duracion = 92, PosterUrl = "/images/peliculas/super_mario.jpg" },
    new Pelicula { Nombre = "Toy Story", Duracion = 81, PosterUrl = "/images/peliculas/toy_story.jpg" }
};

            context.Peliculas.AddRange(peliculas);
            context.SaveChanges();

            // =====================
            // PELICULA_GENERO
            // (Asigno un género cualquiera para que no falle relaciones)
            // =====================
            context.PeliculaGeneros.AddRange(
                new PeliculaGenero { Pelicula = peliculas[0], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[1], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[2], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[3], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[4], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[5], Genero = genero2 },
                new PeliculaGenero { Pelicula = peliculas[6], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[7], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[8], Genero = genero4 },
                new PeliculaGenero { Pelicula = peliculas[9], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[10], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[11], Genero = genero2 },
                new PeliculaGenero { Pelicula = peliculas[12], Genero = genero1 },
                new PeliculaGenero { Pelicula = peliculas[13], Genero = genero2 },
                new PeliculaGenero { Pelicula = peliculas[14], Genero = genero2 }
            );
            context.SaveChanges();

            // =====================
            // PROYECCIONES (7 días × 6 horarios × todas las salas × todas las películas)
            // =====================

            // Horarios estándar de cine
            var horarios = new List<TimeSpan>
{
    new TimeSpan(13, 00, 00),
    new TimeSpan(15, 30, 00),
    new TimeSpan(18, 00, 00),
    new TimeSpan(20, 30, 00),
    new TimeSpan(22, 30, 00),
    new TimeSpan(00, 10, 00) // función trasnoche
};

            // Solo para ejemplo usamos las salas del cine1, cine2 y cine3
            var todasLasSalas = context.Salas.ToList();

            // Limpio proyecciones previas (si relanzas la seed en la misma BD)
            if (context.Proyecciones.Any())
            {
                context.Proyecciones.RemoveRange(context.Proyecciones);
                context.SaveChanges();
            }

            var proyeccionesMasivas = new List<Proyeccion>();

            foreach (var peli in peliculas) // todas las películas
            {
                for (int dia = 0; dia < 7; dia++) // 7 días
                {
                    var fecha = DateTime.Today.AddDays(dia);

                    foreach (var sala in todasLasSalas) // todas las salas disponibles
                    {
                        foreach (var h in horarios) // 6 horarios por día
                        {
                            proyeccionesMasivas.Add(new Proyeccion
                            {
                                Pelicula = peli,
                                Sala = sala,
                                Fecha = fecha.Date,
                                Hora = fecha.Date + h
                            });
                        }
                    }
                }
            }

            context.Proyecciones.AddRange(proyeccionesMasivas);
            context.SaveChanges();
            var proyecciones = context.Proyecciones.ToList();


            // =====================
            // ESPECTADORES
            // =====================
            var espect1 = new Espectador { Dni = 40123123, Email = "espectador1@example.com", Password = "password123" };
            var espect2 = new Espectador { Dni = 39222444, Email = "espectador2@example.com", Password = "password123" };
            var espect3 = new Espectador { Dni = 41555777, Email = "espectador3@example.com", Password = "password123" };
            context.Espectadores.AddRange(espect1, espect2, espect3);
            context.SaveChanges();

            // =====================
            // ENTRADAS
            // =====================
            context.Entradas.AddRange(
    new Entrada { Proyeccion = proyecciones[0], Espectador = espect1, Asiento = asientos[0] },
    new Entrada { Proyeccion = proyecciones[1], Espectador = espect2, Asiento = asientos[4] },
    new Entrada { Proyeccion = proyecciones[2], Espectador = espect3, Asiento = asientos[6] }
);
            context.SaveChanges();

            // =====================
            // CONSUMIBLES
            // =====================
            var c1 = new Consumible { Nombre = "Pochoclos", Descripcion = "Pochoclos salados tamaño grande", Precio = 3500 };
            var c2 = new Consumible { Nombre = "Gaseosa", Descripcion = "Coca-Cola 500ml", Precio = 2500 };
            var c3 = new Consumible { Nombre = "Nachos", Descripcion = "Con queso cheddar", Precio = 3000 };
            var c4 = new Consumible { Nombre = "Agua", Descripcion = "Agua mineral 500ml", Precio = 1800 };
            var c5 = new Consumible { Nombre = "Chocolate", Descripcion = "Barra de chocolate 80g", Precio = 2200 };
            context.Consumibles.AddRange(c1, c2, c3, c4, c5);
            context.SaveChanges();

            // =====================
            // CINE_CONSUMIBLE
            // =====================
            context.CineConsumibles.AddRange(
                new CineConsumible { Cine = cine1, Consumible = c1, Stock = 200 },
                new CineConsumible { Cine = cine1, Consumible = c2, Stock = 150 },
                new CineConsumible { Cine = cine1, Consumible = c3, Stock = 100 },
                new CineConsumible { Cine = cine2, Consumible = c1, Stock = 180 },
                new CineConsumible { Cine = cine2, Consumible = c4, Stock = 120 },
                new CineConsumible { Cine = cine3, Consumible = c1, Stock = 160 },
                new CineConsumible { Cine = cine3, Consumible = c2, Stock = 130 },
                new CineConsumible { Cine = cine3, Consumible = c5, Stock = 90 }
            );
            context.SaveChanges();

            // =====================
            // ESPECTADOR_CONSUMIBLE
            // =====================
            context.EspectadorConsumibles.AddRange(
                new EspectadorConsumible { Espectador = espect1, Consumible = c1, Cine = cine1, Cantidad = 2, Fecha = new DateTime(2025, 10, 30) },
                new EspectadorConsumible { Espectador = espect1, Consumible = c2, Cine = cine1, Cantidad = 1, Fecha = new DateTime(2025, 10, 30) },
                new EspectadorConsumible { Espectador = espect2, Consumible = c4, Cine = cine2, Cantidad = 1, Fecha = new DateTime(2025, 10, 30) },
                new EspectadorConsumible { Espectador = espect3, Consumible = c3, Cine = cine3, Cantidad = 2, Fecha = new DateTime(2025, 10, 30) }
            );
            context.SaveChanges();
        }
    }
}
