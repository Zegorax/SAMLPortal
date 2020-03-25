using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAMLPortal.Migrations
{
    public partial class Setup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Setup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(nullable: true),
                    IsConfigured = table.Column<bool>(nullable: false),
                    IsInMaintenance = table.Column<bool>(nullable: false),
                    LdapHost = table.Column<string>(nullable: true),
                    BindDn = table.Column<string>(nullable: true),
                    BindPass = table.Column<string>(nullable: true),
                    SearchBase = table.Column<string>(nullable: true),
                    UsersFilter = table.Column<string>(nullable: true),
                    AdministratorsFilter = table.Column<string>(nullable: true),
                    UidAttr = table.Column<string>(nullable: true),
                    MemberOfAttr = table.Column<string>(nullable: true),
                    DisplayNameAttr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setup", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Setup");
        }
    }
}
