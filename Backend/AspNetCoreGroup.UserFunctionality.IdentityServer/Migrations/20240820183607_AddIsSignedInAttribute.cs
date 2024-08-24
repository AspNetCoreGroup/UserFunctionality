using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSignedInAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSignedIn",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSignedIn",
                table: "AspNetUsers");
        }
    }
}
