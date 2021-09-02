using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class CreatedProduct3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userID",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_userID",
                table: "Likes",
                column: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_applicationuserlike",
                table: "Likes",
                column: "userID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationuserlike",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_userID",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "userID",
                table: "Likes");
        }
    }
}
