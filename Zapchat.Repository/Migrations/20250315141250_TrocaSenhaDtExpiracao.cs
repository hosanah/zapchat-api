using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zapchat.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TrocaSenhaDtExpiracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataExpiracao",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TrocarSenha",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataExpiracao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TrocarSenha",
                table: "Usuarios");
        }
    }
}
