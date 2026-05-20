using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddKaizenProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KaizenProposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterTaskId = table.Column<int>(type: "int", nullable: false),
                    CurrentSituation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImprovementIdea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaizenProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaizenProposals_MasterTasks_MasterTaskId",
                        column: x => x.MasterTaskId,
                        principalTable: "MasterTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KaizenProposals_MasterTaskId",
                table: "KaizenProposals",
                column: "MasterTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KaizenProposals");
        }
    }
}
