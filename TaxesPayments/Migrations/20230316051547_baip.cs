using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxesPayments.Migrations
{
    /// <inheritdoc />
    public partial class baip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "add",
                table: "Taxes");

            migrationBuilder.RenameColumn(
                name: "msg",
                table: "Taxes",
                newName: "serviceNumber");

            migrationBuilder.RenameColumn(
                name: "err",
                table: "Taxes",
                newName: "sum_usluga");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "Taxes",
                newName: "receipt_id");

            migrationBuilder.RenameColumn(
                name: "akey",
                table: "Taxes",
                newName: "partner_id");

            migrationBuilder.AddColumn<string>(
                name: "accountComment",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "baip_message",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "baip_status",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cashregister_id",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "errMessage",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accountComment",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "baip_message",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "baip_status",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "cashregister_id",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "errMessage",
                table: "Taxes");

            migrationBuilder.RenameColumn(
                name: "sum_usluga",
                table: "Taxes",
                newName: "err");

            migrationBuilder.RenameColumn(
                name: "serviceNumber",
                table: "Taxes",
                newName: "msg");

            migrationBuilder.RenameColumn(
                name: "receipt_id",
                table: "Taxes",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "partner_id",
                table: "Taxes",
                newName: "akey");

            migrationBuilder.AddColumn<double>(
                name: "add",
                table: "Taxes",
                type: "float",
                nullable: true);
        }
    }
}
