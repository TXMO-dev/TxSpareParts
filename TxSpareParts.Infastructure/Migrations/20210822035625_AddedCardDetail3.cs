using Microsoft.EntityFrameworkCore.Migrations;

namespace TxSpareParts.Infastructure.Migrations
{
    public partial class AddedCardDetail3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cart Total",
                table: "ShoppingCarts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShoppingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderTotal",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Is Card Saved",
                table: "CardDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cart Total",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "OrderTotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Is Card Saved",
                table: "CardDetails");
        }
    }
}
