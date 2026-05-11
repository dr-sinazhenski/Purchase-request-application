using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequesterProducts_Products_ProductId",
                table: "RequesterProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterProducts_Requests_RequestId",
                table: "RequesterProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterProducts_Products_ProductId",
                table: "RequesterProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterProducts_Requests_RequestId",
                table: "RequesterProducts",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequesterProducts_Products_ProductId",
                table: "RequesterProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterProducts_Requests_RequestId",
                table: "RequesterProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterProducts_Products_ProductId",
                table: "RequesterProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterProducts_Requests_RequestId",
                table: "RequesterProducts",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
