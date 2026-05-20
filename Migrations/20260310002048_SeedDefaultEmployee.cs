using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KaizenProposals_MasterTasks_MasterTaskId",
                table: "KaizenProposals");

            migrationBuilder.RenameColumn(
                name: "MasterTaskId",
                table: "KaizenProposals",
                newName: "EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_KaizenProposals_MasterTaskId",
                table: "KaizenProposals",
                newName: "IX_KaizenProposals_EmployeeID");

            migrationBuilder.AddColumn<string>(
                name: "Safety",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Satisfaction",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Savings",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Standardization",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sustainability",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                });

            migrationBuilder.Sql("INSERT INTO Employees (Name, Email, Department, Role, FirstLoginDate, LastLoginDate) VALUES ('System', 'system@iars.com', 'IT', 'Admin', GETDATE(), GETDATE());");
            migrationBuilder.Sql("DECLARE @DefaultEmpID INT; SELECT @DefaultEmpID = EmployeeID FROM Employees WHERE Name = 'System'; UPDATE KaizenProposals SET EmployeeID = @DefaultEmpID WHERE EmployeeID IS NULL OR EmployeeID NOT IN (SELECT EmployeeID FROM Employees);");

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_EmployeeID",
                table: "History",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_KaizenProposals_Employees_EmployeeID",
                table: "KaizenProposals",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KaizenProposals_Employees_EmployeeID",
                table: "KaizenProposals");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropColumn(
                name: "Safety",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Satisfaction",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Savings",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Standardization",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Sustainability",
                table: "KaizenProposals");

            migrationBuilder.RenameColumn(
                name: "EmployeeID",
                table: "KaizenProposals",
                newName: "MasterTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_KaizenProposals_EmployeeID",
                table: "KaizenProposals",
                newName: "IX_KaizenProposals_MasterTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_KaizenProposals_MasterTasks_MasterTaskId",
                table: "KaizenProposals",
                column: "MasterTaskId",
                principalTable: "MasterTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
