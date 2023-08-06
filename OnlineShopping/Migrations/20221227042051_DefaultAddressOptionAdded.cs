using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShopping.Migrations
{
    public partial class DefaultAddressOptionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "BillingAddresses",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.Now);

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "BillingAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "BillingAddresses",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.Now);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "BillingAddresses");

            migrationBuilder.DropColumn(
                name: "Default",
                table: "BillingAddresses");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "BillingAddresses");
        }
    }
}
