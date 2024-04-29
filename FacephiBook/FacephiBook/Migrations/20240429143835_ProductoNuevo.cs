using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacephiBook.Migrations
{
    public partial class ProductoNuevo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pixeles",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "Imagen",
                table: "Productos",
                newName: "Gama");

            migrationBuilder.AddColumn<bool>(
                name: "Foco",
                table: "Productos",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PixelFrontal",
                table: "Productos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PixelTrasera",
                table: "Productos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ResCamara",
                table: "Productos",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ResVideo",
                table: "Productos",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foco",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PixelFrontal",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PixelTrasera",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ResCamara",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ResVideo",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "Gama",
                table: "Productos",
                newName: "Imagen");

            migrationBuilder.AddColumn<int>(
                name: "Pixeles",
                table: "Productos",
                type: "int",
                nullable: true);
        }
    }
}
