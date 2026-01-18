using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Overclocked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentWebhooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "payments",
                newName: "amount");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "payments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "payments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.CreateTable(
                name: "payment_webhooks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    created_on_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    error_log = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_webhooks", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payments_payment_method",
                table: "payments",
                column: "payment_method");

            migrationBuilder.CreateIndex(
                name: "ix_payments_status_id",
                table: "payments",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_webhooks_processed_on_utc_created_on_utc",
                table: "payment_webhooks",
                columns: new[] { "processed_on_utc", "created_on_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_webhooks");

            migrationBuilder.DropIndex(
                name: "ix_payments_payment_method",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_status_id",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "payments",
                newName: "price");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "payments",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "payments",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_at",
                table: "orders",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "orders",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }
    }
}
