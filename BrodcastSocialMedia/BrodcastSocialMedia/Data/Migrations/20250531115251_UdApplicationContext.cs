using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrodcastSocialMedia.Data.Migrations
{
    /// <inheritdoc />
    public partial class UdApplicationContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Broadcasts_AspNetUsers_UserId",
                table: "Broadcasts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Broadcasts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BroadcastCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Broadcasts_AspNetUsers_UserId",
                table: "Broadcasts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Broadcasts_AspNetUsers_UserId",
                table: "Broadcasts");

            migrationBuilder.DropColumn(
                name: "BroadcastCount",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Broadcasts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Broadcasts_AspNetUsers_UserId",
                table: "Broadcasts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
