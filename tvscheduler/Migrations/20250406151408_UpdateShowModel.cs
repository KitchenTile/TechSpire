using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvscheduler.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShowModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResizedImageUrl",
                table: "Shows",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResizedImageUrl",
                table: "Shows");
        }
    }
}
