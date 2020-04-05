using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SAMLPortal.Migrations
{
	public partial class Init : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "App",
				columns : table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
						Name = table.Column<string>(nullable: false),
						Description = table.Column<string>(nullable: false),
						Enabled = table.Column<bool>(nullable: false),
						MetadataURL = table.Column<string>(nullable: true),
						Issuer = table.Column<string>(nullable: true),
						SingleSignOnDestination = table.Column<string>(nullable: true),
						SingleLogoutResponseDestination = table.Column<string>(nullable: true),
						SignatureValidationCertificate = table.Column<string>(nullable: true)
				},
				constraints : table =>
				{
					table.PrimaryKey("PK_App", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "KeyValue",
				columns : table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
						Key = table.Column<string>(nullable: false),
						Value = table.Column<string>(nullable: false)
				},
				constraints : table =>
				{
					table.PrimaryKey("PK_KeyValue", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "App");

			migrationBuilder.DropTable(
				name: "KeyValue");
		}
	}
}