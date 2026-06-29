using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Trang.Api.EF
{
    /// <inheritdoc />
    public partial class updatev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProviderId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProviderId",
                table: "OrderDetails",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Providers_ProviderId",
                table: "OrderDetails",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Providers_ProviderId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProviderId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "OrderDetails");
        }
    }
}
