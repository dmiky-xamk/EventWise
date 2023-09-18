using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventWise.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCancelledToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Event",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Event");
        }
    }
}
