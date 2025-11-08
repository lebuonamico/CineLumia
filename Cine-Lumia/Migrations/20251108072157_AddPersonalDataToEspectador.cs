using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Lumia.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalDataToEspectador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Espectadores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Espectadores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Espectadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Espectadores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Espectadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Espectadores");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Espectadores");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Espectadores");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Espectadores");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Espectadores");
        }
    }
}
