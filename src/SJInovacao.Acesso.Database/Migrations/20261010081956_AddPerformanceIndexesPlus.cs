using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SJInovacao.Acesso.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexesPlus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_Name",
                table: "GroupPermissions",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupPermissions_Name",
                table: "GroupPermissions");
        }
    }
}
