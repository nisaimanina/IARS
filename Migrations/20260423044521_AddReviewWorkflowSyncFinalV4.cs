using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewWorkflowSyncFinalV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Safe Column Additions
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Status' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD Status nvarchar(max) NOT NULL DEFAULT ''; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignedHODID' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD AssignedHODID int NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FinalApproverEmployeeID' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD FinalApproverEmployeeID int NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HODRemarks' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD HODRemarks nvarchar(max) NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FinalRemarks' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD FinalRemarks nvarchar(max) NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HODReviewedAt' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD HODReviewedAt datetime2 NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FinalApprovedAt' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD FinalApprovedAt datetime2 NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'ReleasedAt' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD ReleasedAt datetime2 NULL; END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'IsWinner' AND Object_ID = OBJECT_ID(N'KaizenProposals')) BEGIN ALTER TABLE KaizenProposals ADD IsWinner bit NOT NULL DEFAULT 0; END");

            // 2. Indexes and Foreign Keys (using catch to ignore if already exists)
            try { 
                migrationBuilder.CreateIndex(name: "IX_KaizenProposals_AssignedHODID", table: "KaizenProposals", column: "AssignedHODID"); 
            } catch { }
            try { 
                migrationBuilder.CreateIndex(name: "IX_KaizenProposals_FinalApproverEmployeeID", table: "KaizenProposals", column: "FinalApproverEmployeeID"); 
            } catch { }

            try { 
                migrationBuilder.AddForeignKey(name: "FK_KaizenProposals_Employees_AssignedHODID", table: "KaizenProposals", column: "AssignedHODID", principalTable: "Employees", principalColumn: "EmployeeID");
            } catch { }
            try { 
                migrationBuilder.AddForeignKey(name: "FK_KaizenProposals_Employees_FinalApproverEmployeeID", table: "KaizenProposals", column: "FinalApproverEmployeeID", principalTable: "Employees", principalColumn: "EmployeeID");
            } catch { }

            // 3. Seed Demo Users
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM Employees WHERE Email = 'admin@iars.com') INSERT INTO Employees (Name, Email, Department, Role, FirstLoginDate, LastLoginDate) VALUES ('Admin User', 'admin@iars.com', 'Management', 'KaizenCommittee', GETDATE(), GETDATE());");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM Employees WHERE Email = 'ali@iars.com') INSERT INTO Employees (Name, Email, Department, Role, FirstLoginDate, LastLoginDate) VALUES ('Ali', 'ali@iars.com', 'Management Integrated System', 'User', GETDATE(), GETDATE());");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM Employees WHERE Email = 'ahmad@iars.com') INSERT INTO Employees (Name, Email, Department, Role, FirstLoginDate, LastLoginDate) VALUES ('Ahmad', 'ahmad@iars.com', 'Management Integrated System', 'Reviewer', GETDATE(), GETDATE());");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM Employees WHERE Email = 'john@iars.com') INSERT INTO Employees (Name, Email, Department, Role, FirstLoginDate, LastLoginDate) VALUES ('John', 'john@iars.com', 'Management Integrated System', 'FinalApprover', GETDATE(), GETDATE());");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KaizenProposals_Employees_AssignedHODID",
                table: "KaizenProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_KaizenProposals_Employees_FinalApproverEmployeeID",
                table: "KaizenProposals");

            migrationBuilder.DropIndex(
                name: "IX_KaizenProposals_AssignedHODID",
                table: "KaizenProposals");

            migrationBuilder.DropIndex(
                name: "IX_KaizenProposals_FinalApproverEmployeeID",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "AssignedHODID",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "FinalApprovedAt",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "FinalApproverEmployeeID",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "FinalRemarks",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "HODRemarks",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "HODReviewedAt",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ReleasedAt",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "KaizenProposals");
        }
    }
}
