using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClayAccessControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoorStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Doors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Doors");
        }
    }
}
