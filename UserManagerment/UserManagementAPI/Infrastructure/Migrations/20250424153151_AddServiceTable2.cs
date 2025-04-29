using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_ApplicationServices_ServicesId",
                table: "ApplicationUserServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UsersId",
                table: "ApplicationUserServices");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserServices_ServicesId",
                table: "ApplicationUserServices");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserServices_UsersId",
                table: "ApplicationUserServices");

            migrationBuilder.DropColumn(
                name: "ServicesId",
                table: "ApplicationUserServices");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "ApplicationUserServices");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ApplicationUserServices",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceId",
                table: "ApplicationUserServices",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_ServiceId",
                table: "ApplicationUserServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_UserId",
                table: "ApplicationUserServices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_ApplicationServices_ServiceId",
                table: "ApplicationUserServices",
                column: "ServiceId",
                principalTable: "ApplicationServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UserId",
                table: "ApplicationUserServices",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_ApplicationServices_ServiceId",
                table: "ApplicationUserServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UserId",
                table: "ApplicationUserServices");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserServices_ServiceId",
                table: "ApplicationUserServices");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserServices_UserId",
                table: "ApplicationUserServices");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ApplicationUserServices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceId",
                table: "ApplicationUserServices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ServicesId",
                table: "ApplicationUserServices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "ApplicationUserServices",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_ServicesId",
                table: "ApplicationUserServices",
                column: "ServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_UsersId",
                table: "ApplicationUserServices",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_ApplicationServices_ServicesId",
                table: "ApplicationUserServices",
                column: "ServicesId",
                principalTable: "ApplicationServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserServices_AspNetUsers_UsersId",
                table: "ApplicationUserServices",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
