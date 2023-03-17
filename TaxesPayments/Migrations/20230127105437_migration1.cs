using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxesPayments.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "accountName",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "carNumber",
                table: "Taxes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sum",
                table: "Taxes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accountName",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "carNumber",
                table: "Taxes");

            migrationBuilder.DropColumn(
                name: "sum",
                table: "Taxes");
        }
    }
}
