using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Extensions;
using OnlineShopping.Models;

#nullable disable

namespace OnlineShopping.Migrations
{
    public partial class UserRoleAddedToUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: Roles.User.GetDisplayName());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
