using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend___Putka.Migrations
{
    public partial class BannersTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSmall",
                table: "Banners",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSmall",
                table: "Banners");
        }
    }
}
