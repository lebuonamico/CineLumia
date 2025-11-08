using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Lumia.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToNombreCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Espectadores");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Espectadores");

            migrationBuilder.AddColumn<string>(
                name: "NombreCompleto",
                table: "Espectadores",
                type: "nvarchar(201)",
                maxLength: 201,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreCompleto",
                table: "Espectadores");

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Espectadores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Espectadores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
