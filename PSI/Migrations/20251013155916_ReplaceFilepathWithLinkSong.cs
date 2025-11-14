using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSI.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFilepathWithLinkSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Songs",
                newName: "Link");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Songs",
                newName: "FilePath");
        }
    }
}
