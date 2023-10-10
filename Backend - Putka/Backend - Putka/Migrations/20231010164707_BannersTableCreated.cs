using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend___Putka.Migrations
{
    public partial class BannersTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopText = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    HeaderOne = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    SaleText = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    HeaderTwo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BackgroundImage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
