using Cine_Lumia.Entities;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Data
{
    public static class CineSeeder
    {
        public static void Seed(CineDbContext context)
        {
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
            // FORMATOS
            // =====================
            var formato2D = new Formato { Nombre = "2D" };
            var formato3D = new Formato { Nombre = "3D" };
            var formato4D = new Formato { Nombre = "4D" };
            var formatoXD = new Formato { Nombre = "XD" };
            context.Formato.AddRange(formato2D, formato3D, formato4D, formatoXD);
            context.SaveChanges();

            // =====================
            // SALAS
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
            // ASIENTOS
            // =====================
            var asientos = new List<Asiento>();
            var salas = new[] { sala1, sala2, sala3, sala4, sala5, sala6 };
            foreach (var s in salas)
            {
                for (int f = 1; f <= s.Cant_Filas; f++)
                {
                    for (int c = 1; c <= s.Cant_Butacas; c++)
                    {
                        asientos.Add(new Asiento
                        {
                            Fila = ((char)('A' + f - 1)).ToString(),
                            Columna = c,
                            Disponible = true,
                            Sala = s
                        });
                    }
                }
            }
            context.Asientos.AddRange(asientos);
            context.SaveChanges();

            // =====================
            // TIPOS DE ENTRADA
            // =====================
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
                new Pelicula { Nombre = "The Dark Knight", Duracion = 152, PosterUrl = "https://upload.wikimedia.org/wikipedia/en/8/8a/Dark_Knight.jpg" },
                new Pelicula { Nombre = "Dune", Duracion = 155, PosterUrl = "/images/peliculas/dune.webp" },
                new Pelicula { Nombre = "Frozen", Duracion = 102, PosterUrl = "/images/peliculas/frozen.jpg" }
            };
            context.Peliculas.AddRange(peliculas);
            context.SaveChanges();

            // =====================
            // PELICULA-GENERO
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
            var baseHorarios = new List<TimeSpan>
{
    new TimeSpan(8, 0, 0),
    new TimeSpan(15, 45, 0),
    new TimeSpan(18, 15, 0),
    new TimeSpan(23, 30, 0) // todas las salas deben tener esta
};

            var proyecciones = new List<Proyeccion>();
            var random = new Random();

            foreach (var cine in context.Cines.Include(c => c.Salas))
            {
                int salaIndex = 0;

                foreach (var peli in peliculas)
                {
                    for (int d = 0; d < 7; d++)
                    {
                        var fecha = DateTime.Today.AddDays(d);

                        foreach (var sala in cine.Salas)
                        {
                            // Variación según sala (0 a 45 min) para los horarios que NO son 23:30
                            int variacion = (salaIndex * 15) % 60; // 15 min entre salas
                            var horarios = baseHorarios
                                .Select(h =>
                                    h == new TimeSpan(23, 30, 0)
                                        ? h // la última siempre igual
                                        : h.Add(TimeSpan.FromMinutes(variacion)))
                                .ToList();

                            foreach (var h in horarios)
                            {
                                proyecciones.Add(new Proyeccion
                                {
                                    Pelicula = peli,
                                    Sala = sala,
                                    Fecha = fecha,
                                    Hora = h
                                });
                            }

                            salaIndex++;
                        }
                    }
                }
            }

            context.Proyecciones.AddRange(proyecciones);
            context.SaveChanges();



            // =====================
            // ESPECTADORES
            // =====================
            var espect1 = new Espectador { Nombre = "Juan", Apellido = "Perez", Telefono = "1155485654", Dni = "40123123", Email = "espectador1@example.com", Password = "password123" };
            var espect2 = new Espectador { Nombre = "Maria", Apellido = "Gomez", Telefono = "1155485654", Dni = "39222444", Email = "espectador2@example.com", Password = "password123" };
            var espect3 = new Espectador { Nombre = "Pedro", Apellido = "Rodriguez", Telefono = "1155485654", Dni = "41555777", Email = "espectador3@example.com", Password = "password123" };
            context.Espectadores.AddRange(espect1, espect2, espect3);
            context.SaveChanges();

            // =====================
            // ENTRADAS (proyecciones de 23:30 → dejar 10 libres aleatorios y dispersos)
            // =====================
            var entradas = new List<Entrada>();
            var random2 = new Random();

            foreach (var p in proyecciones.Where(p => p.Hora.Hours == 23 && p.Hora.Minutes == 30))
            {
                var sala = context.Salas.Include(s => s.Asientos).FirstOrDefault(s => s.Id_Sala == p.Id_Sala);
                if (sala == null) continue;

                var asientosSala = context.Asientos
                    .Where(a => a.Id_Sala == sala.Id_Sala)
                    .OrderBy(a => a.Fila)
                    .ThenBy(a => a.Columna)
                    .ToList();

                int totalAsientos = asientosSala.Count;
                int libres = 30;

                // 1️⃣ Elijo 10 asientos distintos para dejar libres
                var indicesLibres = new HashSet<int>();
                while (indicesLibres.Count < libres)
                {
                    indicesLibres.Add(random2.Next(0, totalAsientos));
                }

                // 2️⃣ Genero algunos "bloques" de dispersión (simular agrupaciones)
                int cantidadBloques = random2.Next(1, 5); // de 1 a 4 grupos
                var asientosLibresExtra = new List<int>();

                for (int b = 0; b < cantidadBloques; b++)
                {
                    var baseIndex = random2.Next(0, totalAsientos);
                    int bloqueSize = random2.Next(1, 3); // tamaño 1 o 2
                    for (int i = 0; i < bloqueSize; i++)
                    {
                        int idx = baseIndex + i;
                        if (idx < totalAsientos)
                            asientosLibresExtra.Add(idx);
                    }
                }

                // Combino ambos métodos y me aseguro de tener 10 libres únicos
                var asientosLibres = indicesLibres
                    .Concat(asientosLibresExtra)
                    .Distinct()
                    .Take(libres)
                    .ToList();

                // 3️⃣ Los demás se marcan como ocupados
                foreach (var (asiento, index) in asientosSala.Select((a, i) => (a, i)))
                {
                    if (!asientosLibres.Contains(index))
                    {
                        entradas.Add(new Entrada
                        {
                            Proyeccion = p,
                            Espectador = espect1,
                            Asiento = asiento,
                            Id_TipoEntrada = context.TipoEntrada
                                .First(t => t.Id_Formato == sala.Id_Formato)
                                .Id_TipoEntrada
                        });
                    }
                }
            }

            context.Entradas.AddRange(entradas);
            context.SaveChanges();

            // =====================
            // CONSUMIBLES
            // =====================
            // =====================
            // CINE_CONSUMIBLE simplificado
            // =====================
            // =====================
            // CONSUMIBLES - Lote 1 simplificado
            // =====================
    if (!context.Consumibles.Any(c => c.Nombre.StartsWith("COMBO"))) // Comprobación más específica
    {
        // Usar un inicializador de colección para agregar todos los objetos de una sola vez
        var nuevosConsumibles = new List<Consumible>
        {
            new Consumible { Nombre = "COMBO TOY STORY 30 ANIVERSARIO", Descripcion = "1 Vaso metálico \"Toy Story 30 Aniversario\" (sin contenido) + 1 vaso de gaseosa grande + 1 Balde de pochoclos. UNIDADES LIMITADAS.", Precio = 44900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004243.png?v=00002639" },
            new Consumible { Nombre = "COMBO TRON: ARES", Descripcion = "1 ()Vaso \"Tron: Ares\" con gaseosa + 1 Balde de pochoclos. ()Con Luz LED. UNIDADES LIMITADAS.", Precio = 44900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004261PR.png?v=00002639" },
            new Consumible { Nombre = "COMBO PITUFOS", Descripcion = "1 Pochoclera \"Pitufos\" con pochoclos + 1 gaseosa grande. UNIDADES LIMITADAS.", Precio = 24900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004216.png?v=00002639" },
            new Consumible { Nombre = "COMBO EL CONJURO 4", Descripcion = "1 Vaso \"El Conjuro 4\" con gaseosa + 1 Balde de pochoclos. UNIDADES LIMITADAS.", Precio = 24900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004236.png?v=00002639" },
            new Consumible { Nombre = "COMBO SUPERMAN", Descripcion = "1 ()Vaso \"Superman\" con gaseosa + 1 balde de pochoclos. UNIDADES LIMITADAS. ()Tiene luz LED.", Precio = 24900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004207.png?v=00002639" },
            new Consumible { Nombre = "COMBO LOS 4 FANTASTICOS", Descripcion = "1 ()Vaso \"Los 4 Fantásticos: Primeros pasos\" sin contenido + 1 vaso de gaseosa + 1 balde de pochoclos. ()4 modelos disponibles a eleccion en el cine. UNIDADES LIMITADAS.", Precio = 24900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004213.png?v=00002639" },
            new Consumible { Nombre = "RECARGA VASO REUTILIZABLE", Descripcion = "1 Recarga de gaseosa de 910ml., válida exclusivamente para el Vaso Reutilizable Cinemark-Hoyts. Recordá llevar tu vaso al cine.", Precio = 2900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004011.png?v=00002639" },
            new Consumible { Nombre = "COMBO COMO ENTRENAR A TU DRAGON", Descripcion = "1 Vaso \"Cómo Entrenar a tu Dragón\" con gaseosa + 1 balde de pochoclos. UNIDADES LIMITADAS.", Precio = 48900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004192.png?v=00002639" },
            new Consumible { Nombre = "COMBO GABBY´S DOLLHOUSE", Descripcion = "1 ()Vaso \"Gabbys DollHouse\" con gaseosa + 1 Balde de pochoclos. ()2 modelos disponibles a eleccion en el cine. UNIDADES LIMITADAS.", Precio = 34900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004253.png?v=00002639" },
            new Consumible { Nombre = "COMBO MEGA EL CONJURO 4", Descripcion = "1 Vaso \"El Conjuro 4\" sin contenido + 1 Balde de pochoclos + 2 gaseosas grandes. UNIDADES LIMITADAS.", Precio = 29900, PosterUrl = "https://static.cinemarkhoyts.com.ar/Images/ConcessionItemImageN/A000004272.png?v=00002639" },
        };

        context.Consumibles.AddRange(nuevosConsumibles);
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }


            // =====================
            // CINE_CONSUMIBLE para Lote 1 - Usando Single Save
            // =====================

            if (!context.CineConsumibles.Any())
            {
                // Obtener los consumibles recién creados o existentes
                var consumiblesLote1 = context.Consumibles.AsNoTracking().ToList();
                // Obtener los cines de la base de datos para asegurar que existan
                var cinesLote = context.Cines.AsNoTracking().ToList();

                if(cinesLote.Any() && consumiblesLote1.Any())
                {
                    // Creamos todas las relaciones CineConsumible en una sola lista
                    var todasLasRelaciones = cinesLote
                        .SelectMany(cine => consumiblesLote1
                            .Select(consumible => new CineConsumible
                            {
                                // Usamos las propiedades ID del cine y consumible para evitar problemas de tracking
                                Id_Cine = cine.Id_Cine,
                                Id_Consumible = consumible.Id_Consumible,
                                Stock = 200
                            }))
                        .ToList();

                    // Agregamos todas las relaciones a la vez
                    context.CineConsumibles.AddRange(todasLasRelaciones);
                    context.SaveChanges();
                    context.ChangeTracker.Clear();
                }
            }

        }
    }
}
