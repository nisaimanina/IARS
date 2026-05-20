using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IARS.Migrations
{
    /// <inheritdoc />
    public partial class AddFormNoToKaizen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormNo",
                table: "KaizenProposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormNo",
                table: "KaizenProposals");
        }
    }
}
