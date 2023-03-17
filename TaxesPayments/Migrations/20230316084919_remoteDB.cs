using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxesPayments.Migrations
{
    /// <inheritdoc />
    public partial class remoteDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "serviceNumber",
                table: "Taxes",
                newName: "baip_serviceNumber");

            migrationBuilder.RenameColumn(
                name: "receipt_id",
                table: "Taxes",
                newName: "baip_receipt_id");

            migrationBuilder.RenameColumn(
                name: "partner_id",
                table: "Taxes",
                newName: "baip_partner_id");

            migrationBuilder.RenameColumn(
                name: "cashregister_id",
                table: "Taxes",
                newName: "baip_cashregister_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "baip_serviceNumber",
                table: "Taxes",
                newName: "serviceNumber");

            migrationBuilder.RenameColumn(
                name: "baip_receipt_id",
                table: "Taxes",
                newName: "receipt_id");

            migrationBuilder.RenameColumn(
                name: "baip_partner_id",
                table: "Taxes",
                newName: "partner_id");

            migrationBuilder.RenameColumn(
                name: "baip_cashregister_id",
                table: "Taxes",
                newName: "cashregister_id");
        }
    }
}
