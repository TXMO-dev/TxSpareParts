using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class AddedCardDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserID",
                table: "CompanyReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_ApplicationUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Products_ProductId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Stars_AspNetUsers_ApplicationUserId",
                table: "Stars");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Stars",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Stars_ApplicationUserId",
                table: "Stars",
                newName: "IX_Stars_UserID");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ShoppingCarts",
                newName: "ProductID");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "ShoppingCarts",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_ProductId",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_ApplicationUserId",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_UserID");

            migrationBuilder.AddColumn<string>(
                name: "EmailForCard",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CardDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccountName = table.Column<string>(name: "Account Name", type: "nvarchar(max)", nullable: true),
                    transactionEmail = table.Column<string>(name: "transaction Email", type: "nvarchar(max)", nullable: true),
                    AuthorizationCode = table.Column<string>(name: "Authorization Code", type: "nvarchar(max)", nullable: true),
                    CardType = table.Column<string>(name: "Card Type", type: "nvarchar(max)", nullable: true),
                    Last4digitsofcard = table.Column<string>(name: "Last 4 digits of card", type: "nvarchar(max)", nullable: true),
                    ExpiryMonth = table.Column<string>(name: "Expiry Month", type: "nvarchar(max)", nullable: true),
                    ExpiryYear = table.Column<string>(name: "Expiry Year", type: "nvarchar(max)", nullable: true),
                    Bin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentChannel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reusable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_carddetailapplicationuser",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardDetails_UserId",
                table: "CardDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_companyreviewsinuser",
                table: "CompanyReviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingcartappuser",
                table: "ShoppingCarts",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingcartforproduct",
                table: "ShoppingCarts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_alluserstarred",
                table: "Stars",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_companyreviewsinuser",
                table: "CompanyReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_shoppingcartappuser",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_shoppingcartforproduct",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_alluserstarred",
                table: "Stars");

            migrationBuilder.DropTable(
                name: "CardDetails");

            migrationBuilder.DropColumn(
                name: "EmailForCard",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Stars",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Stars_UserID",
                table: "Stars",
                newName: "IX_Stars_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "ShoppingCarts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "ShoppingCarts",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_ProductID",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_UserID",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyReviews_AspNetUsers_UserID",
                table: "CompanyReviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_ApplicationUserId",
                table: "ShoppingCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Products_ProductId",
                table: "ShoppingCarts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stars_AspNetUsers_ApplicationUserId",
                table: "Stars",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
