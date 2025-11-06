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
            // SALAS
            // =====================
            var sala1 = new Sala { Cant_Butacas = 10, Cant_Filas = 10, Capacidad = 100, Cine = cine1 };
            var sala2 = new Sala { Cant_Butacas = 10, Cant_Filas = 8, Capacidad = 80, Cine = cine1 };
            var sala3 = new Sala { Cant_Butacas = 12, Cant_Filas = 10, Capacidad = 120, Cine = cine2 };
            var sala4 = new Sala { Cant_Butacas = 8, Cant_Filas = 8, Capacidad = 64, Cine = cine2 };
            var sala5 = new Sala { Cant_Butacas = 15, Cant_Filas = 10, Capacidad = 150, Cine = cine3 };
            context.Salas.AddRange(sala1, sala2, sala3, sala4, sala5);
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
            var peli1 = new Pelicula { Nombre = "Ecos del Pasado", Duracion = 120, Fecha_Estreno = new DateTime(2025, 10, 1) };
            var peli2 = new Pelicula { Nombre = "Risas en el Metro", Duracion = 95, Fecha_Estreno = new DateTime(2025, 8, 15) };
            var peli3 = new Pelicula { Nombre = "Sombras de Medianoche", Duracion = 110, Fecha_Estreno = new DateTime(2025, 9, 10) };
            context.Peliculas.AddRange(peli1, peli2, peli3);
            context.SaveChanges();

            // =====================
            // PELICULA_GENERO
            // =====================
            context.PeliculaGeneros.AddRange(
                new PeliculaGenero { Pelicula = peli1, Genero = genero4 },
                new PeliculaGenero { Pelicula = peli2, Genero = genero2 },
                new PeliculaGenero { Pelicula = peli3, Genero = genero3 }
            );
            context.SaveChanges();

            // =====================
            // PROYECCIONES
            // =====================
            var proy1 = new Proyeccion { Fecha = new DateTime(2025, 10, 30), Hora = new DateTime(2025, 10, 30, 19, 30, 0), Sala = sala1, Pelicula = peli1 };
            var proy2 = new Proyeccion { Fecha = new DateTime(2025, 10, 30), Hora = new DateTime(2025, 10, 30, 21, 0, 0), Sala = sala2, Pelicula = peli2 };
            var proy3 = new Proyeccion { Fecha = new DateTime(2025, 10, 30), Hora = new DateTime(2025, 10, 30, 22, 15, 0), Sala = sala3, Pelicula = peli3 };
            context.Proyecciones.AddRange(proy1, proy2, proy3);
            context.SaveChanges();

            // =====================
            // ESPECTADORES
            // =====================
            var espect1 = new Espectador { Dni = "40123123", Email = "espectador1@example.com", Password = "password123" };
            var espect2 = new Espectador { Dni = "39222444", Email = "espectador2@example.com", Password = "password123" };
            var espect3 = new Espectador { Dni = "41555777", Email = "espectador3@example.com", Password = "password123" };
            context.Espectadores.AddRange(espect1, espect2, espect3);
            context.SaveChanges();

            // =====================
            // ENTRADAS
            // =====================
            context.Entradas.AddRange(
                new Entrada { Proyeccion = proy1, Espectador = espect1, Asiento = asientos[0] },
                new Entrada { Proyeccion = proy2, Espectador = espect2, Asiento = asientos[4] },
                new Entrada { Proyeccion = proy3, Espectador = espect3, Asiento = asientos[6] }
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
