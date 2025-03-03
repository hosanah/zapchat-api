using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zapchat.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPlataformaToGrupoWhatsApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Plataforma",
                table: "GruposWhatsApp",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plataforma",
                table: "GruposWhatsApp");
        }
    }
}
