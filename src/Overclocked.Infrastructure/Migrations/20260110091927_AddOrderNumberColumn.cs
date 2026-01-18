using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Overclocked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "order_number",
                table: "orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_orders_order_number",
                table: "orders",
                column: "order_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_orders_order_number",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "order_number",
                table: "orders");
        }
    }
}
