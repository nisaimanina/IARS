using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesToKaizen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageAfterPath",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageBeforePath",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAfterPath",
                table: "KaizenProposals");

            migrationBuilder.DropColumn(
                name: "ImageBeforePath",
                table: "KaizenProposals");
        }
    }
}
