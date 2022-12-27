using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWProject.Migrations
{
    /// <inheritdoc />
    public partial class AlcoholType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlcoholType",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholType",
                table: "Products");
        }
    }
}
