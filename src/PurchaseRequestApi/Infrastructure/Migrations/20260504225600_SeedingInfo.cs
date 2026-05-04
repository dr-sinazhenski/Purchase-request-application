using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedingInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Currency", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "USD", "North America" },
                    { new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), "EUR", "Europe" },
                    { new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), "EUR", "Lithuania" }
                });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "ProductId", "RegionId", "Amount", "UnitsOfMeasure" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 12.00m, "pack" },
                    { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 10.00m, "pack" },
                    { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 9.00m, "pack" },
                    { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 8.00m, "pack" },
                    { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 7.00m, "pack" },
                    { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 6.50m, "pack" },
                    { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 150.00m, "license" },
                    { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 140.00m, "license" },
                    { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 135.00m, "license" },
                    { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 600.00m, "license" },
                    { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 560.00m, "license" },
                    { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 540.00m, "license" },
                    { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 80.00m, "license" },
                    { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 75.00m, "license" },
                    { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 70.00m, "license" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 1200.00m, "pcs" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 1100.00m, "pcs" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 1050.00m, "pcs" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 300.00m, "pcs" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 280.00m, "pcs" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 270.00m, "pcs" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 45.00m, "pcs" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 40.00m, "pcs" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 38.00m, "pcs" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 25.00m, "pcs" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 22.00m, "pcs" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 20.00m, "pcs" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 350.00m, "pcs" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 320.00m, "pcs" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 300.00m, "pcs" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 400.00m, "pcs" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), 370.00m, "pcs" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"), 350.00m, "pcs" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.DeleteData(
                table: "Prices",
                keyColumns: new[] { "ProductId", "RegionId" },
                keyValues: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") });

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd"));
        }
    }
}
