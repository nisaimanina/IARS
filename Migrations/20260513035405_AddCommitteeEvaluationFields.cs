using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeEvaluationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckedBy",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedDate",
                table: "KaizenProposals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommitteeRank",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCommitteeKaizen",
                table: "KaizenProposals",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "KaizenProposals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreApplication",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreEffort",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreIdea",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreImprovement",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreSafety",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreTangible",
                table: "KaizenProposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalCommitteeScore",
                table: "KaizenProposals",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedBy",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "CheckedDate",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "CommitteeRank",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsCommitteeKaizen",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreApplication",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreEffort",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreIdea",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreImprovement",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreSafety",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ScoreTangible",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "TotalCommitteeScore",
                table: "KaizenProposals");
        }
    }
}
