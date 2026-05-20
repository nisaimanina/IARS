using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class Add8ImpactCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sustainability",
                table: "KaizenProposals",
                newName: "VendorSupport");

            migrationBuilder.RenameColumn(
                name: "Standardization",
                table: "KaizenProposals",
                newName: "Quality");

            migrationBuilder.RenameColumn(
                name: "Savings",
                table: "KaizenProposals",
                newName: "InvestmentCost");

            migrationBuilder.RenameColumn(
                name: "Satisfaction",
                table: "KaizenProposals",
                newName: "FiveS");

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Delivery",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Digital",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "Digital",
                table: "KaizenProposals");

            migrationBuilder.RenameColumn(
                name: "VendorSupport",
                table: "KaizenProposals",
                newName: "Sustainability");

            migrationBuilder.RenameColumn(
                name: "Quality",
                table: "KaizenProposals",
                newName: "Standardization");

            migrationBuilder.RenameColumn(
                name: "InvestmentCost",
                table: "KaizenProposals",
                newName: "Savings");

            migrationBuilder.RenameColumn(
                name: "FiveS",
                table: "KaizenProposals",
                newName: "Satisfaction");
        }
    }
}
