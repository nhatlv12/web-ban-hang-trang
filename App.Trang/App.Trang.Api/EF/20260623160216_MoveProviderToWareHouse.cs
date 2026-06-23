using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Trang.Api.EF
{
    /// <inheritdoc />
    public partial class MoveProviderToWareHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Providers_ProviderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProviderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "WareHouses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WareHouses_ProviderId",
                table: "WareHouses",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_WareHouses_Providers_ProviderId",
                table: "WareHouses",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WareHouses_Providers_ProviderId",
                table: "WareHouses");

            migrationBuilder.DropIndex(
                name: "IX_WareHouses_ProviderId",
                table: "WareHouses");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "WareHouses");

            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProviderId",
                table: "Products",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Providers_ProviderId",
                table: "Products",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
