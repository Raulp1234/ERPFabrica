using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPFabrica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class M1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecuenciasNumeros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Año = table.Column<int>(type: "int", nullable: false),
                    UltimoNumero = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuenciasNumeros", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecuenciasNumeros_TenantId_Entidad_Año",
                table: "SecuenciasNumeros",
                columns: new[] { "TenantId", "Entidad", "Año" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecuenciasNumeros");
        }
    }
}
