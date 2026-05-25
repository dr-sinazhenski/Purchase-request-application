using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AccountId", "CreationTime", "RequestId", "Text" },
                values: new object[,]
                {
                    { new Guid("11111111-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new DateTime(2025, 2, 1, 9, 5, 0, 0, DateTimeKind.Utc), new Guid("dddddddd-4444-4444-4444-444444444444"), "The design team currently uses outdated monitors which are affecting productivity." },
                    { new Guid("22222222-5555-5555-5555-555555555555"), new Guid("cccccccc-3333-3333-3333-333333333333"), new DateTime(2025, 2, 2, 11, 0, 0, 0, DateTimeKind.Utc), new Guid("dddddddd-4444-4444-4444-444444444444"), "Please provide model specifications and price quotes before we can proceed." },
                    { new Guid("33333333-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new DateTime(2025, 2, 3, 16, 0, 0, 0, DateTimeKind.Utc), new Guid("dddddddd-4444-4444-4444-444444444444"), "Resubmitted with full specifications attached. Requesting Dell 27\" 4K monitors." },
                    { new Guid("aaaaaaaa-5555-5555-5555-555555555555"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), new DateTime(2025, 1, 10, 9, 5, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-4444-4444-4444-444444444444"), "Requesting a high-performance laptop for running multiple development environments." },
                    { new Guid("bbbbbbbb-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new DateTime(2025, 1, 11, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-4444-4444-4444-444444444444"), "Please specify the required RAM and storage capacity." },
                    { new Guid("cccccccc-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new DateTime(2025, 1, 15, 10, 5, 0, 0, DateTimeKind.Utc), new Guid("bbbbbbbb-4444-4444-4444-444444444444"), "Monthly restock as usual, same quantities as last month." },
                    { new Guid("dddddddd-5555-5555-5555-555555555555"), new Guid("cccccccc-3333-3333-3333-333333333333"), new DateTime(2025, 1, 16, 14, 0, 0, 0, DateTimeKind.Utc), new Guid("bbbbbbbb-4444-4444-4444-444444444444"), "Approved. Supplies will be delivered by end of week." },
                    { new Guid("eeeeeeee-5555-5555-5555-555555555555"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), new DateTime(2025, 1, 20, 8, 5, 0, 0, DateTimeKind.Utc), new Guid("cccccccc-4444-4444-4444-444444444444"), "We need these licenses renewed before the end of January to avoid service interruptions." },
                    { new Guid("ffffffff-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new DateTime(2025, 1, 22, 11, 0, 0, 0, DateTimeKind.Utc), new Guid("cccccccc-4444-4444-4444-444444444444"), "Rejected. Budget for software licenses has been frozen for Q1. Please resubmit in Q2." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("11111111-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("22222222-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("33333333-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-5555-5555-5555-555555555555"));
        }
    }
}
