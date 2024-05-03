using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacephiBook.Migrations
{
    public partial class ReservasUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devoluciones_Reservas_ReservaId",
                table: "Devoluciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Reservas_ReservaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Productos_ProductoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_IdUsuario",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Reservas_ReservaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ReservaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_IdUsuario",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ReservaId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Devoluciones_ReservaId",
                table: "Devoluciones");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Devoluciones");

            migrationBuilder.DropColumn(
                name: "IdReserva",
                table: "Devoluciones");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Devoluciones",
                newName: "UsuarioId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Hora",
                table: "Reservas",
                type: "time",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DevolucionId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservaId1",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_DevolucionId",
                table: "Reservas",
                column: "DevolucionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ReservaId1",
                table: "Productos",
                column: "ReservaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_ReservaId",
                table: "Devoluciones",
                column: "ReservaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devoluciones_Reservas_ReservaId",
                table: "Devoluciones",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Reservas_ReservaId1",
                table: "Productos",
                column: "ReservaId1",
                principalTable: "Reservas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Devoluciones_DevolucionId",
                table: "Reservas",
                column: "DevolucionId",
                principalTable: "Devoluciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Productos_ProductoId",
                table: "Reservas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devoluciones_Reservas_ReservaId",
                table: "Devoluciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Reservas_ReservaId1",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Devoluciones_DevolucionId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Productos_ProductoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_DevolucionId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ReservaId1",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Devoluciones_ReservaId",
                table: "Devoluciones");

            migrationBuilder.DropColumn(
                name: "DevolucionId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ReservaId1",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Devoluciones",
                newName: "IdUsuario");

            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Reservas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Hora",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Devoluciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdReserva",
                table: "Devoluciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ReservaId",
                table: "Usuarios",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdUsuario",
                table: "Reservas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ReservaId",
                table: "Productos",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Devoluciones_ReservaId",
                table: "Devoluciones",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devoluciones_Reservas_ReservaId",
                table: "Devoluciones",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Reservas_ReservaId",
                table: "Productos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Productos_ProductoId",
                table: "Reservas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_IdUsuario",
                table: "Reservas",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Reservas_ReservaId",
                table: "Usuarios",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id");
        }
    }
}
