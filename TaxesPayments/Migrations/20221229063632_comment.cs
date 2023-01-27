using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxesPayments.Migrations
{
    /// <inheritdoc />
    public partial class comment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    point = table.Column<int>(type: "int", nullable: false),
                    skey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    service = table.Column<int>(type: "int", nullable: false),
                    account = table.Column<int>(type: "int", nullable: false),
                    persacc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    akey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    err = table.Column<int>(type: "int", nullable: false),
                    msg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    add = table.Column<double>(type: "float", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxes", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Taxes");
        }
    }
}
