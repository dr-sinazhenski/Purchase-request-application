using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixedSeededRequests : Migration
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

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "ApproverProfileId", "Login", "Name", "Password", "RegionId" },
                values: new object[] { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), null, "john.doe", "John Doe", "hashed_password_1", new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "ApproverProfiles",
                columns: new[] { "Id", "MaxAmount", "MinAmount", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), 500m, 0m, "Junior Approver" },
                    { new Guid("bbbbbbbb-1111-1111-1111-111111111111"), 2000m, 500m, "Senior Approver" },
                    { new Guid("cccccccc-1111-1111-1111-111111111111"), 999999m, 2000m, "Executive Approver" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "Requester" },
                    { new Guid("bbbbbbbb-2222-2222-2222-222222222222"), "Approver" },
                    { new Guid("cccccccc-2222-2222-2222-222222222222"), "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AccountRole",
                columns: new[] { "AccountId", "RoleId" },
                values: new object[] { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-2222-2222-2222-222222222222") });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "ApproverProfileId", "Login", "Name", "Password", "RegionId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("bbbbbbbb-1111-1111-1111-111111111111"), "jane.smith", "Jane Smith", "hashed_password_2", new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") },
                    { new Guid("cccccccc-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "peter.jones", "Peter Jones", "hashed_password_3", new Guid("cccccccc-dddd-dddd-dddd-dddddddddddd") },
                    { new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("cccccccc-1111-1111-1111-111111111111"), "admin", "System Admin", "hashed_password_4", new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc") }
                });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Id", "CreatedAt", "Description", "RejectionCommentId", "RejectionCommentId1", "RequestTypeId", "RequesterId", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-4444-4444-4444-444444444444"), new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), "Requesting a new laptop for the development team", null, null, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), 0, "New Laptop for Development", new DateTime(2025, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cccccccc-4444-4444-4444-444444444444"), new DateTime(2025, 1, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Annual software license renewal for Q1", null, null, new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-3333-3333-3333-333333333333"), 3, "Software Licenses Q1", new DateTime(2025, 1, 22, 11, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AccountRole",
                columns: new[] { "AccountId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-2222-2222-2222-222222222222") },
                    { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("bbbbbbbb-2222-2222-2222-222222222222") },
                    { new Guid("cccccccc-3333-3333-3333-333333333333"), new Guid("bbbbbbbb-2222-2222-2222-222222222222") },
                    { new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("cccccccc-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "RequesterProducts",
                columns: new[] { "ProductId", "RequestId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-4444-4444-4444-444444444444"), 1 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-4444-4444-4444-444444444444"), 1 },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("aaaaaaaa-4444-4444-4444-444444444444"), 1 },
                    { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-4444-4444-4444-444444444444"), 3 },
                    { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-4444-4444-4444-444444444444"), 3 }
                });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Id", "CreatedAt", "Description", "RejectionCommentId", "RejectionCommentId1", "RequestTypeId", "RequesterId", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-4444-4444-4444-444444444444"), new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Monthly office supplies restock for the Vilnius office", null, null, new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), 2, "Office Supplies Restock", new DateTime(2025, 1, 16, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dddddddd-4444-4444-4444-444444444444"), new DateTime(2025, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Requesting monitor upgrades for the design team", null, null, new Guid("11111111-1111-1111-1111-111111111111"), new Guid("bbbbbbbb-3333-3333-3333-333333333333"), 1, "Monitor Upgrade", new DateTime(2025, 2, 3, 16, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "RequesterProducts",
                columns: new[] { "ProductId", "RequestId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-4444-4444-4444-444444444444"), 5 },
                    { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-4444-4444-4444-444444444444"), 3 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("dddddddd-4444-4444-4444-444444444444"), 2 }
                });

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

            migrationBuilder.DeleteData(
                table: "AccountRole",
                keyColumns: new[] { "AccountId", "RoleId" },
                keyValues: new object[] { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "AccountRole",
                keyColumns: new[] { "AccountId", "RoleId" },
                keyValues: new object[] { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "AccountRole",
                keyColumns: new[] { "AccountId", "RoleId" },
                keyValues: new object[] { new Guid("bbbbbbbb-3333-3333-3333-333333333333"), new Guid("bbbbbbbb-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "AccountRole",
                keyColumns: new[] { "AccountId", "RoleId" },
                keyValues: new object[] { new Guid("cccccccc-3333-3333-3333-333333333333"), new Guid("bbbbbbbb-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "AccountRole",
                keyColumns: new[] { "AccountId", "RoleId" },
                keyValues: new object[] { new Guid("dddddddd-3333-3333-3333-333333333333"), new Guid("cccccccc-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("aaaaaaaa-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("aaaaaaaa-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("cccccccc-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "RequesterProducts",
                keyColumns: new[] { "ProductId", "RequestId" },
                keyValues: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("dddddddd-4444-4444-4444-444444444444") });

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "ApproverProfiles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "ApproverProfiles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "ApproverProfiles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-1111-1111-1111-111111111111"));

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
