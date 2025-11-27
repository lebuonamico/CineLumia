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
                new Pelicula { Nombre = "The Dark Knight", Duracion = 152, PosterUrl = "/images/peliculas/• the-dark-knight.jpg" },
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
new Consumible { Nombre = "Agua", Descripcion = "1 Agua. Disponibilidad de marca según el cine, sujeto a disponibilidad.", Precio = 4200, PosterUrl = "/images/snacks/BE_A000000013.png" },
new Consumible { Nombre = "Bebida Mediana", Descripcion = "1 Gaseosa 21 oz.", Precio = 7500, PosterUrl = "/images/snacks/BE_A000000027.png" },
new Consumible { Nombre = "Bebida Grande", Descripcion = "1 gaseosa 24 oz.", Precio = 8400, PosterUrl = "/images/snacks/BE_A000000028.png" },
new Consumible { Nombre = "Smoothie Limonada", Descripcion = "1 Smoothie Limonada.", Precio = 8400, PosterUrl = "/images/snacks/BE_A000001399.png" },
new Consumible { Nombre = "Agua Saborizada", Descripcion = "1 Agua saborizada. Eleccion del sabor en el cine, sujeto a disponibilidad.", Precio = 5000, PosterUrl = "/images/snacks/BE_A000001427.png" },
new Consumible { Nombre = "Lata Antares HONEY 473CC", Descripcion = "1 Lata Antares Honey 473cc.- presentar DNI.", Precio = 8900, PosterUrl = "/images/snacks/BE_A000004169.png" },
new Consumible { Nombre = "Lata Antares IPA 473CC", Descripcion = "1 Lata Antares IPA 473cc.- presentar DNI.", Precio = 8900, PosterUrl = "/images/snacks/BE_A000004175.png" },
new Consumible { Nombre = "Cafe Snack Grande", Descripcion = "1 Cafe 16 oz.", Precio = 7300, PosterUrl = "/images/snacks/CAF_A000002501.png" },
new Consumible { Nombre = "Bombones Franui.png", Descripcion = "Valor por unidad. Sabores: Chocolate amargo - chocolate con leche.", Precio = 12900, PosterUrl = "/images/snacks/CAF_A000002786.png" },
new Consumible { Nombre = "Bebida Mediana + Pan de Queso", Descripcion = "1 bebida mediana + 1 porción de pan de queso (5 unidades)", Precio = 11900, PosterUrl = "/images/snacks/CAF_A000002832.png" },
new Consumible { Nombre = "Cafe Snack Grande + Pan de Queso", Descripcion = "1 café grande + 1 porción de pan de queso (5 unidades)", Precio = 11900, PosterUrl = "/images/snacks/CAF_A000002933.png" },
new Consumible { Nombre = "Cafe Snack Grande + Alfajor", Descripcion = "1 Cafe 16 oz + 1 Alfajor. Imagen a modo ilustrativo.", Precio = 8415, PosterUrl = "/images/snacks/CAF_A000003627.png" },
new Consumible { Nombre = "Cafe Snack Grande + Cofler Choco Cookies", Descripcion = "1 Cafe 16 oz + 1 Cofler Choco Cookies. Dos variedades: Oreo - Bon o Bon.", Precio = 10030, PosterUrl = "/images/snacks/CAF_A000004157.png" },
new Consumible { Nombre = "Combo Mega", Descripcion = "1 Balde de pochoclos + 2 gaseosas grandes + 1 golosina sin eleccion (sujeta a disponibilidad del cine).", Precio = 24210, PosterUrl = "/images/snacks/CO_A000000196.png" },
new Consumible { Nombre = "Combo Mega Individual", Descripcion = "1 balde de pochoclos + 1 gaseosa grande.", Precio = 17370, PosterUrl = "/images/snacks/CO_A000000197.png" },
new Consumible { Nombre = "Combo Familia", Descripcion = "2 Baldes de pochoclos + 4 gaseosas medianas + 2 golosinas sin eleccion + 1 agua.", Precio = 39510, PosterUrl = "/images/snacks/CO_A000000198.png" },
new Consumible { Nombre = "Combo Mega Recargado", Descripcion = "1 Balde pop+2 gaseosas grandes + 1 golosina +1 recarga* de balde + 1 recarga* - 00:00 y 2:00 AM", Precio = 28125, PosterUrl = "/images/snacks/CO_A000000199.png" },
new Consumible { Nombre = "Combo Pancho ", Descripcion = "1 Pancho + 1 gaseosa grande.", Precio = 10625, PosterUrl = "/images/snacks/CO_A000000203.png" },
new Consumible { Nombre = "Combo Smoothie Limonada", Descripcion = "1 Smoothie limonada + 1 pop envasado.", Precio = 10965, PosterUrl = "/images/snacks/CO_A000001817.png" },
new Consumible { Nombre = "Combo Pop Envasado", Descripcion = "1 Pop envasado* + 1 gaseosa grande. *Producto sin tacc, sabor dulce. Exclusivo Cinemark-Hoyts.", Precio = 10965, PosterUrl = "/images/snacks/CO_A000001838.png" },
new Consumible { Nombre = "Combo Nuggets de Pollo (x8)", Descripcion = "Porcion de 8 unidades + porcion de papas fritas + 1 gaseosa grande.", Precio = 15130, PosterUrl = "/images/snacks/CO_A000003461.png" },
new Consumible { Nombre = "Combo Nachos", Descripcion = "Nachos con queso + 1 gaseosa grande.", Precio = 13345, PosterUrl = "/images/snacks/CO_A000003790.png" },
new Consumible { Nombre = "Papas con Cheddar + Lata Antares HONEY 473CC", Descripcion = "Papas con cheddar + 1 lata Antares Honey 473cc. - presentar DNI.", Precio = 16575, PosterUrl = "/images/snacks/CO_A000004171.png" },
new Consumible { Nombre = "Nachos Ahumados + Lata Antares HONEY 473CC", Descripcion = "Nachos ahumados con queso + 1 Lata Antares Honey 473cc.- presentar DNI.", Precio = 14875, PosterUrl = "/images/snacks/CO_A000004174.png" },
new Consumible { Nombre = "Papas con Cheddar + Lata Antares IPA 473CC", Descripcion = "Papas con cheddar + 1 lata Antares IPA 473cc. - presentar DNI.", Precio = 16575, PosterUrl = "/images/snacks/CO_A000004177.png" },
new Consumible { Nombre = "Tequeños + Lata Antares IPA 473CC", Descripcion = "1 Porcion de tequeños (4 unidades) + salsa tártara + 1 lata Antares IPA 473cc. - presentar DNI.", Precio = 16575, PosterUrl = "/images/snacks/CO_A000004178.png" },
new Consumible { Nombre = "Nachos Ahumados + Lata Antares IPA 473CC", Descripcion = "Nachos ahumados con queso + 1 Lata Antares IPA 473cc.- presentar DNI.", Precio = 14875, PosterUrl = "/images/snacks/CO_A000004179.png" },
new Consumible { Nombre = "Combo Pop Envasado Cookies & Cream", Descripcion = "1 Pop envasado* + 1 gaseosa grande. *Producto sin tacc, sabor cookies & cream.", Precio = 11220, PosterUrl = "/images/snacks/CO_A000004182.png" },
new Consumible { Nombre = "Combo Pop Envasado Cookies & Cream + M&M Mantequilla Mani", Descripcion = "1 Pop envasado* + 1 gaseosa grande + M&M Mantequilla de Maní.", Precio = 15725, PosterUrl = "/images/snacks/CO_A000004230.png" },
new Consumible { Nombre = "Combo Pop Envasado Cookies & Cream + M&M Caramelo", Descripcion = "1 Pop envasado* + 1 gaseosa grande + M&M Caramelo.", Precio = 15725, PosterUrl = "/images/snacks/CO_A000004231.png" },
new Consumible { Nombre = "Recarga Vaso Reutilizable", Descripcion = "1 Recarga de gaseosa de 910ml, válida exclusivamente para el Vaso Reutilizable. Recordá llevar tu vaso al cine.", Precio = 2900, PosterUrl = "/images/snacks/COL_A000004011.png" },
new Consumible { Nombre = "Combo Como Entrenar a tu Dragon", Descripcion = "1 Vaso Cómo Entrenar a tu Dragón con gaseosa + 1 balde de pochoclos. UNIDADES LIMITADAS.", Precio = 48900, PosterUrl = "/images/snacks/COL_A000004192.png" },
new Consumible { Nombre = "Combo Superman", Descripcion = "1 (*)Vaso Superman con gaseosa + 1 balde de pochoclos. UNIDADES LIMITADAS. (*)Tiene luz LED.", Precio = 24900, PosterUrl = "/images/snacks/COL_A000004207.png" },
new Consumible { Nombre = "Combo Los 4 FANTASTICOS", Descripcion = "1 (*)Vaso Los 4 Fantásticos: Primeros pasos  sin contenido + 1 vaso de gaseosa + 1 Balde.", Precio = 24900, PosterUrl = "/images/snacks/COL_A000004213.png" },
new Consumible { Nombre = "Combo El Conjuro 4", Descripcion = "1 Vaso El Conjuro 4 con gaseosa + 1 Balde de pochoclos. UNIDADES LIMITADAS.", Precio = 24900, PosterUrl = "/images/snacks/COL_A000004236.png" },
new Consumible { Nombre = "Combo Toy Story 30 ANIVERSARIO", Descripcion = "1 Vaso metálico Toy Story 30 Aniversario (sin contenido) +1 vaso de gaseosa grande + 1 Balde", Precio = 44900, PosterUrl = "/images/snacks/COL_A000004243.png" },
new Consumible { Nombre = "Combo Gabby's Dollhouse", Descripcion = "1 (*)Vaso Gabbys DollHouse con gaseosa + 1 Balde de pochoclos. UNIDADES LIMITADAS.", Precio = 34900, PosterUrl = "/images/snacks/COL_A000004253.png" },
new Consumible { Nombre = "Alfajor", Descripcion = "1 Alfajor. Disponibilidad de marca según el cine. Valor por unidad.", Precio = 3700, PosterUrl = "/images/snacks/GO_A000000067.png" },
new Consumible { Nombre = "SUGUS caramelos Confitados", Descripcion = "1 Caramelos confitados.", Precio = 3200, PosterUrl = "/images/snacks/GO_A000000071.png" },
new Consumible { Nombre = "Rocklets Chicos", Descripcion = "1 Rocklets chico.", Precio = 4200, PosterUrl = "/images/snacks/GO_A000000072.png" },
new Consumible { Nombre = "Rocklets Grandes", Descripcion = "1 Rocklets grandes.", Precio = 10500, PosterUrl = "/images/snacks/GO_A000000073.png" },
new Consumible { Nombre = "M&M Grande", Descripcion = "1 m&m grande. Modelo sujeto a disponibilidad.", Precio = 11000, PosterUrl = "/images/snacks/GO_A000000081.png" },
new Consumible { Nombre = "M&M Chico", Descripcion = "1 m&m chico. Elección del sabor sujeto a disponibilidad.", Precio = 6200, PosterUrl = "/images/snacks/GO_A000000082.png" },
new Consumible { Nombre = "Skittles Medianos", Descripcion = "1 Skittles mediano.", Precio = 6200, PosterUrl = "/images/snacks/GO_A000000085.png" },
new Consumible { Nombre = "Skittles Grandes", Descripcion = "1 Skittles 204 grs.", Precio = 11000, PosterUrl = "/images/snacks/GO_A000001989.png" },
new Consumible { Nombre = "Gomitas HARIBO", Descripcion = "1 Gomitas Haribo. Imagen a modo ilustrativo. Sabor sujeto a disponibilidad.", Precio = 6500, PosterUrl = "/images/snacks/GO_A000002547.png" },
new Consumible { Nombre = "Cofler Choco Cookies", Descripcion = "1 Cofler Choco Cookies (105 gr). Dos variedades: Oreo / Bon o Bon. Valor por unidad.", Precio = 6200, PosterUrl = "/images/snacks/GO_A000003961.png" },
new Consumible { Nombre = "Choco Pause", Descripcion = "1 chocolate Milka Pause sabor Oreo o Leche. Valor por unidad.", Precio = 3700, PosterUrl = "/images/snacks/GO_A000004153.png" },
new Consumible { Nombre = "MOGUL Moras Chico", Descripcion = "1 mogul Valor por unidad.", Precio = 5000, PosterUrl = "/images/snacks/GO_A000004197.png" },
new Consumible { Nombre = "M&M Caramelo", Descripcion = "1 m&m Caramelo.", Precio = 5900, PosterUrl = "/images/snacks/GO_A000004209.png" },
new Consumible { Nombre = "M&M Mantequilla de Mani", Descripcion = "1 m&m Caramelo Mantequilla de Mani", Precio = 5900, PosterUrl = "/images/snacks/GO_A000004210.png" },
new Consumible { Nombre = "Pop Mediano", Descripcion = "1 Bolsa de pochoclos mediana.", Precio = 8500, PosterUrl = "/images/snacks/PO_A000000010.png" },
new Consumible { Nombre = "Balde de Pochoclos", Descripcion = "1 Balde de pochoclos.", Precio = 10900, PosterUrl = "/images/snacks/PO_A000000011.png" },
new Consumible { Nombre = "Pop Envasado", Descripcion = "1 bolsa de pochoclos envasados*. -Producto sin tacc, sabor dulce.", Precio = 6500, PosterUrl = "/images/snacks/PO_A000001744.png" },
new Consumible { Nombre = "Pop Envasado Cookies & Cream", Descripcion = "1 bolsa de pochoclos envasados*. -Producto sin tacc, sabor cookies & cream.", Precio = 6800, PosterUrl = "/images/snacks/PO_A000004181.png" },
new Consumible { Nombre = "Pancho", Descripcion = "1 Pancho.", Precio = 5300, PosterUrl = "/images/snacks/SNA_A000000034.png" },
new Consumible { Nombre = "Nachos con Queso", Descripcion = "1 Nachos con queso.", Precio = 9500, PosterUrl = "/images/snacks/SNA_A000000038.png" },
new Consumible { Nombre = "Lays Clasicas", Descripcion = "1 Snack.", Precio = 5000, PosterUrl = "/images/snacks/SNA_A000000066.png" },
new Consumible { Nombre = "Pan de Queso", Descripcion = "1 porción de pan de queso (5 unidades)", Precio = 4500, PosterUrl = "/images/snacks/SNA_A000002831.png" },
new Consumible { Nombre = "Tequeños", Descripcion = "Porcion de tequeños x4 unidades + salsa tártara.", Precio = 12900, PosterUrl = "/images/snacks/SNA_A000003375.png" },
new Consumible { Nombre = "Nuggets de Pollo(X8)", Descripcion = "Porcion de 8 unidades.", Precio = 12900, PosterUrl = "/images/snacks/SNA_A000003467.png" } };

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

                if (cinesLote.Any() && consumiblesLote1.Any())
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
