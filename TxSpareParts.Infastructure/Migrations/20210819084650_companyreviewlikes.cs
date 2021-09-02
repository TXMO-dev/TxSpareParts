using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class companyreviewlikes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserId",
                table: "CompanyReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyReviews_Likes_LikeId",
                table: "CompanyReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyReviews_Products_ProductId",
                table: "CompanyReviews");

            migrationBuilder.DropIndex(
                name: "IX_CompanyReviews_LikeId",
                table: "CompanyReviews");

            migrationBuilder.DropIndex(
                name: "IX_CompanyReviews_ProductId",
                table: "CompanyReviews");

            migrationBuilder.DropColumn(
                name: "LikeId",
                table: "CompanyReviews");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CompanyReviews");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CompanyReviews",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyReviews_UserId",
                table: "CompanyReviews",
                newName: "IX_CompanyReviews_UserID");

            migrationBuilder.CreateTable(
                name: "CompanyReviewLikes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyReviewID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyReviewLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_companycompanyreviewlike",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_companyreviewcomplikes",
                        column: x => x.CompanyReviewID,
                        principalTable: "CompanyReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usercompanyreviewlikes",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviewLikes_CompanyID",
                table: "CompanyReviewLikes",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviewLikes_CompanyReviewID",
                table: "CompanyReviewLikes",
                column: "CompanyReviewID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviewLikes_UserID",
                table: "CompanyReviewLikes",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserID",
                table: "CompanyReviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserID",
                table: "CompanyReviews");

            migrationBuilder.DropTable(
                name: "CompanyReviewLikes");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "CompanyReviews",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyReviews_UserID",
                table: "CompanyReviews",
                newName: "IX_CompanyReviews_UserId");

            migrationBuilder.AddColumn<string>(
                name: "LikeId",
                table: "CompanyReviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "CompanyReviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviews_LikeId",
                table: "CompanyReviews",
                column: "LikeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyReviews_ProductId",
                table: "CompanyReviews",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserId",
                table: "CompanyReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyReviews_Likes_LikeId",
                table: "CompanyReviews",
                column: "LikeId",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyReviews_Products_ProductId",
                table: "CompanyReviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
