using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighbor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrationAddBiography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Accounts");
        }
    }
}
