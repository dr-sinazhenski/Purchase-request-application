using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NamesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EditedAt",
                table: "Requests",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "RejectionCommentId",
                table: "Requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectionCommentId1",
                table: "Requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequesterId",
                table: "Requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RejectionCommentId1",
                table: "Requests",
                column: "RejectionCommentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequesterId",
                table: "Requests",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Accounts_RequesterId",
                table: "Requests",
                column: "RequesterId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Comments_RejectionCommentId1",
                table: "Requests",
                column: "RejectionCommentId1",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Accounts_RequesterId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Comments_RejectionCommentId1",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RejectionCommentId1",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequesterId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RejectionCommentId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RejectionCommentId1",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Requests",
                newName: "EditedAt");
        }
    }
}
