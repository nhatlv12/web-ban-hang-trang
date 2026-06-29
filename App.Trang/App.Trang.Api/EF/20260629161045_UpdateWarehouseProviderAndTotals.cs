using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Trang.Api.EF
{
    /// <inheritdoc />
    public partial class UpdateWarehouseProviderAndTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WareHouses_ProductId",
                table: "WareHouses");

            migrationBuilder.AddColumn<int>(
                name: "TotalExport",
                table: "WareHouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalImport",
                table: "WareHouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WareHouses_ProductId_ProviderId",
                table: "WareHouses",
                columns: new[] { "ProductId", "ProviderId" },
                unique: true,
                filter: "[ProviderId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WareHouses_ProductId_ProviderId",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "TotalExport",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "TotalImport",
                table: "WareHouses");

            migrationBuilder.CreateIndex(
                name: "IX_WareHouses_ProductId",
                table: "WareHouses",
                column: "ProductId",
                unique: true);
        }
    }
}
