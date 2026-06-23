using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientEmployeeID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IconBg = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IconColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Employees_RecipientEmployeeID",
                        column: x => x.RecipientEmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientEmployeeID",
                table: "Notifications",
                column: "RecipientEmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
