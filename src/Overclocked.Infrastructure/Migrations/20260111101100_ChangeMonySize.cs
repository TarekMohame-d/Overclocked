using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Overclocked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMonySize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Status",
                table: "payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount",
                table: "products",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_price_amount",
                table: "orders",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "product_image",
                table: "order_items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Status",
                table: "payments",
                sql: "status_id IN (1, 2, 3, 4, 5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "orders",
                sql: "status_id IN (1, 2, 3, 4, 5, 6, 7, 8, 9)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Status",
                table: "payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "product_image",
                table: "order_items");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount",
                table: "products",
                type: "numeric(2,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)",
                oldPrecision: 8,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "total_price_amount",
                table: "orders",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)",
                oldPrecision: 8,
                oldScale: 2);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Status",
                table: "payments",
                sql: "status_id IN (1, 2, 3, 4, 5, 6, 7)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "orders",
                sql: "status_id IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)");
        }
    }
}
