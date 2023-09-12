using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventWise.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Event",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Event_PublicId",
                table: "Event",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Event_PublicId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Event");
        }
    }
}
