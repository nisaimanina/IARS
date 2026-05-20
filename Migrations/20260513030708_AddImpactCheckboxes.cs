using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddImpactCheckboxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCost",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivery",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDigital",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEngagement",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFiveS",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuality",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSafety",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSatisfaction",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSocialImpact",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSustainability",
                table: "KaizenProposals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCost",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsDelivery",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsDigital",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsEngagement",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsFiveS",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsQuality",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsSafety",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsSatisfaction",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsSocialImpact",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "IsSustainability",
                table: "KaizenProposals");
        }
    }
}
