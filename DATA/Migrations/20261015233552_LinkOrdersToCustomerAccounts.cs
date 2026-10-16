using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATA.Migrations
{
    /// <inheritdoc />
    public partial class LinkOrdersToCustomerAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerAccountId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerAccountId",
                table: "Orders",
                column: "CustomerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CustomerAccounts_CustomerAccountId",
                table: "Orders",
                column: "CustomerAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CustomerAccounts_CustomerAccountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerAccountId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerAccountId",
                table: "Orders");
        }
    }
}
