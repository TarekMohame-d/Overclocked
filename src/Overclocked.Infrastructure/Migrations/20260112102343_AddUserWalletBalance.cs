using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Overclocked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserWalletBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "balance",
                table: "users",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "users",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance",
                table: "users");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "users");
        }
    }
}
