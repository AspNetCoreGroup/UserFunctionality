using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class Telegramm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telegramm",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telegramm",
                table: "AspNetUsers");
        }
    }
}
