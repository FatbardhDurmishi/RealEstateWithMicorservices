using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Services.TransactionService.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionType",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "TransactionTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TransactionType",
                table: "Transactions",
                newName: "IX_Transactions_TransactionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                table: "Transactions",
                column: "TransactionTypeId",
                principalTable: "TransactionTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionTypeId",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TransactionTypeId",
                table: "Transactions",
                newName: "IX_Transactions_TransactionType");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionType",
                table: "Transactions",
                column: "TransactionType",
                principalTable: "TransactionTypes",
                principalColumn: "Id");
        }
    }
}
