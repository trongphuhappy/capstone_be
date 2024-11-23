using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighbor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CropCoverPhotoId",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CropCoverPhotoUrl",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullCoverPhotoId",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullCoverPhotoUrl",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CropCoverPhotoId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CropCoverPhotoUrl",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FullCoverPhotoId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FullCoverPhotoUrl",
                table: "Accounts");
        }
    }
}
