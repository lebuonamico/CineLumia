using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Lumia.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consumibles",
                columns: table => new
                {
                    Id_Consumible = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumibles", x => x.Id_Consumible);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id_Empresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id_Empresa);
                });

            migrationBuilder.CreateTable(
                name: "Espectadores",
                columns: table => new
                {
                    Id_Espectador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdAvatar = table.Column<int>(type: "int", nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alias = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Espectadores", x => x.Id_Espectador);
                });

            migrationBuilder.CreateTable(
                name: "Formato",
                columns: table => new
                {
                    Id_Formato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formato", x => x.Id_Formato);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id_Genero = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneroNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id_Genero);
                });

            migrationBuilder.CreateTable(
                name: "Peliculas",
                columns: table => new
                {
                    Id_Pelicula = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duracion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fecha_Estreno = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peliculas", x => x.Id_Pelicula);
                });

            migrationBuilder.CreateTable(
                name: "Cines",
                columns: table => new
                {
                    Id_Cine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Id_Empresa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cines", x => x.Id_Cine);
                    table.ForeignKey(
                        name: "FK_Cines_Empresas_Id_Empresa",
                        column: x => x.Id_Empresa,
                        principalTable: "Empresas",
                        principalColumn: "Id_Empresa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoEntrada",
                columns: table => new
                {
                    Id_TipoEntrada = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Formato = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEntrada", x => x.Id_TipoEntrada);
                    table.ForeignKey(
                        name: "FK_TipoEntrada_Formato_Id_Formato",
                        column: x => x.Id_Formato,
                        principalTable: "Formato",
                        principalColumn: "Id_Formato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeliculaGeneros",
                columns: table => new
                {
                    Id_Pelicula = table.Column<int>(type: "int", nullable: false),
                    Id_Genero = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculaGeneros", x => new { x.Id_Pelicula, x.Id_Genero });
                    table.ForeignKey(
                        name: "FK_PeliculaGeneros_Generos_Id_Genero",
                        column: x => x.Id_Genero,
                        principalTable: "Generos",
                        principalColumn: "Id_Genero",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculaGeneros_Peliculas_Id_Pelicula",
                        column: x => x.Id_Pelicula,
                        principalTable: "Peliculas",
                        principalColumn: "Id_Pelicula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CineConsumibles",
                columns: table => new
                {
                    Id_Cine = table.Column<int>(type: "int", nullable: false),
                    Id_Consumible = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CineConsumibles", x => new { x.Id_Cine, x.Id_Consumible });
                    table.ForeignKey(
                        name: "FK_CineConsumibles_Cines_Id_Cine",
                        column: x => x.Id_Cine,
                        principalTable: "Cines",
                        principalColumn: "Id_Cine",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CineConsumibles_Consumibles_Id_Consumible",
                        column: x => x.Id_Consumible,
                        principalTable: "Consumibles",
                        principalColumn: "Id_Consumible",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EspectadorConsumibles",
                columns: table => new
                {
                    Id_Compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Espectador = table.Column<int>(type: "int", nullable: false),
                    Id_Consumible = table.Column<int>(type: "int", nullable: false),
                    Id_Cine = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspectadorConsumibles", x => x.Id_Compra);
                    table.ForeignKey(
                        name: "FK_EspectadorConsumibles_Cines_Id_Cine",
                        column: x => x.Id_Cine,
                        principalTable: "Cines",
                        principalColumn: "Id_Cine",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspectadorConsumibles_Consumibles_Id_Consumible",
                        column: x => x.Id_Consumible,
                        principalTable: "Consumibles",
                        principalColumn: "Id_Consumible",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspectadorConsumibles_Espectadores_Id_Espectador",
                        column: x => x.Id_Espectador,
                        principalTable: "Espectadores",
                        principalColumn: "Id_Espectador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salas",
                columns: table => new
                {
                    Id_Sala = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cant_Butacas = table.Column<int>(type: "int", nullable: false),
                    Cant_Filas = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    Id_Cine = table.Column<int>(type: "int", nullable: false),
                    Id_Formato = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salas", x => x.Id_Sala);
                    table.ForeignKey(
                        name: "FK_Salas_Cines_Id_Cine",
                        column: x => x.Id_Cine,
                        principalTable: "Cines",
                        principalColumn: "Id_Cine",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Salas_Formato_Id_Formato",
                        column: x => x.Id_Formato,
                        principalTable: "Formato",
                        principalColumn: "Id_Formato",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asientos",
                columns: table => new
                {
                    Id_Asiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fila = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Columna = table.Column<int>(type: "int", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    Id_Sala = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asientos", x => x.Id_Asiento);
                    table.ForeignKey(
                        name: "FK_Asientos_Salas_Id_Sala",
                        column: x => x.Id_Sala,
                        principalTable: "Salas",
                        principalColumn: "Id_Sala",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proyecciones",
                columns: table => new
                {
                    Id_Proyeccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Id_Sala = table.Column<int>(type: "int", nullable: false),
                    Id_Pelicula = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyecciones", x => x.Id_Proyeccion);
                    table.ForeignKey(
                        name: "FK_Proyecciones_Peliculas_Id_Pelicula",
                        column: x => x.Id_Pelicula,
                        principalTable: "Peliculas",
                        principalColumn: "Id_Pelicula",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proyecciones_Salas_Id_Sala",
                        column: x => x.Id_Sala,
                        principalTable: "Salas",
                        principalColumn: "Id_Sala",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    Id_Entrada = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Proyeccion = table.Column<int>(type: "int", nullable: false),
                    Id_Espectador = table.Column<int>(type: "int", nullable: false),
                    Id_Asiento = table.Column<int>(type: "int", nullable: false),
                    Id_TipoEntrada = table.Column<int>(type: "int", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.Id_Entrada);
                    table.ForeignKey(
                        name: "FK_Entradas_Asientos_Id_Asiento",
                        column: x => x.Id_Asiento,
                        principalTable: "Asientos",
                        principalColumn: "Id_Asiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entradas_Espectadores_Id_Espectador",
                        column: x => x.Id_Espectador,
                        principalTable: "Espectadores",
                        principalColumn: "Id_Espectador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entradas_Proyecciones_Id_Proyeccion",
                        column: x => x.Id_Proyeccion,
                        principalTable: "Proyecciones",
                        principalColumn: "Id_Proyeccion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entradas_TipoEntrada_Id_TipoEntrada",
                        column: x => x.Id_TipoEntrada,
                        principalTable: "TipoEntrada",
                        principalColumn: "Id_TipoEntrada",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asientos_Id_Sala",
                table: "Asientos",
                column: "Id_Sala");

            migrationBuilder.CreateIndex(
                name: "IX_CineConsumibles_Id_Consumible",
                table: "CineConsumibles",
                column: "Id_Consumible");

            migrationBuilder.CreateIndex(
                name: "IX_Cines_Id_Empresa",
                table: "Cines",
                column: "Id_Empresa");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_Id_Asiento",
                table: "Entradas",
                column: "Id_Asiento");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_Id_Espectador",
                table: "Entradas",
                column: "Id_Espectador");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_Id_Proyeccion",
                table: "Entradas",
                column: "Id_Proyeccion");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_Id_TipoEntrada",
                table: "Entradas",
                column: "Id_TipoEntrada");

            migrationBuilder.CreateIndex(
                name: "IX_EspectadorConsumibles_Id_Cine",
                table: "EspectadorConsumibles",
                column: "Id_Cine");

            migrationBuilder.CreateIndex(
                name: "IX_EspectadorConsumibles_Id_Consumible",
                table: "EspectadorConsumibles",
                column: "Id_Consumible");

            migrationBuilder.CreateIndex(
                name: "IX_EspectadorConsumibles_Id_Espectador",
                table: "EspectadorConsumibles",
                column: "Id_Espectador");

            migrationBuilder.CreateIndex(
                name: "IX_PeliculaGeneros_Id_Genero",
                table: "PeliculaGeneros",
                column: "Id_Genero");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecciones_Id_Pelicula",
                table: "Proyecciones",
                column: "Id_Pelicula");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecciones_Id_Sala",
                table: "Proyecciones",
                column: "Id_Sala");

            migrationBuilder.CreateIndex(
                name: "IX_Salas_Id_Cine",
                table: "Salas",
                column: "Id_Cine");

            migrationBuilder.CreateIndex(
                name: "IX_Salas_Id_Formato",
                table: "Salas",
                column: "Id_Formato");

            migrationBuilder.CreateIndex(
                name: "IX_TipoEntrada_Id_Formato",
                table: "TipoEntrada",
                column: "Id_Formato");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CineConsumibles");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "EspectadorConsumibles");

            migrationBuilder.DropTable(
                name: "PeliculaGeneros");

            migrationBuilder.DropTable(
                name: "Asientos");

            migrationBuilder.DropTable(
                name: "Proyecciones");

            migrationBuilder.DropTable(
                name: "TipoEntrada");

            migrationBuilder.DropTable(
                name: "Consumibles");

            migrationBuilder.DropTable(
                name: "Espectadores");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "Peliculas");

            migrationBuilder.DropTable(
                name: "Salas");

            migrationBuilder.DropTable(
                name: "Cines");

            migrationBuilder.DropTable(
                name: "Formato");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
