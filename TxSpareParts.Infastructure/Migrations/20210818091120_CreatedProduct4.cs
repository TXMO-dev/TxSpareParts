using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class CreatedProduct4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Likes_LikeId",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_LikeId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "LikeId",
                table: "ProductReviews");

            migrationBuilder.CreateTable(
                name: "ProductReviewLikes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductReviewID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviewLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productproductreviewlikes",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_theactualproductreview",
                        column: x => x.ProductReviewID,
                        principalTable: "ProductReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userproductreviewlikes",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviewLikes_ProductID",
                table: "ProductReviewLikes",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviewLikes_ProductReviewID",
                table: "ProductReviewLikes",
                column: "ProductReviewID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviewLikes_UserID",
                table: "ProductReviewLikes",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReviewLikes");

            migrationBuilder.AddColumn<string>(
                name: "LikeId",
                table: "ProductReviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_LikeId",
                table: "ProductReviews",
                column: "LikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Likes_LikeId",
                table: "ProductReviews",
                column: "LikeId",
                principalTable: "Likes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
