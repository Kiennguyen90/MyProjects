using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationServices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserServices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ServiceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserServices_ApplicationServices_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "ApplicationServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserServices_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_ServicesId",
                table: "ApplicationUserServices",
                column: "ServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_UsersId",
                table: "ApplicationUserServices",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserServices");

            migrationBuilder.DropTable(
                name: "ApplicationServices");
        }
    }
}
