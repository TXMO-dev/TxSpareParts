using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class productreviewupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_AspNetUsers_UserId",
                table: "ProductReviews");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ProductReviews",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                newName: "IX_ProductReviews_UserID");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ProductReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Updated",
                table: "ProductReviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_applicationuserproductreview",
                table: "ProductReviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationuserproductreview",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Date_Updated",
                table: "ProductReviews");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "ProductReviews",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_UserID",
                table: "ProductReviews",
                newName: "IX_ProductReviews_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_AspNetUsers_UserId",
                table: "ProductReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
