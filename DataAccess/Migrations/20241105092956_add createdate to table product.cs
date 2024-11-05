using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class addcreatedatetotableproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Product",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2024, 11, 5, 16, 29, 56, 377, DateTimeKind.Local).AddTicks(4371));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2024, 11, 5, 16, 29, 56, 377, DateTimeKind.Local).AddTicks(6136),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2024, 11, 5, 16, 26, 30, 433, DateTimeKind.Local).AddTicks(4623));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Product");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2024, 11, 5, 16, 26, 30, 433, DateTimeKind.Local).AddTicks(4623),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2024, 11, 5, 16, 29, 56, 377, DateTimeKind.Local).AddTicks(6136));
        }
    }
}
