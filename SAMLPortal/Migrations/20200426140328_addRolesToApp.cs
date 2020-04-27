using Microsoft.EntityFrameworkCore.Migrations;

namespace SAMLPortal.Migrations
{
	public partial class addRolesToApp : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "SingleSignOnDestination",
				table: "App",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "longtext CHARACTER SET utf8mb4",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "SingleLogoutResponseDestination",
				table: "App",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "longtext CHARACTER SET utf8mb4",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "Issuer",
				table: "App",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "longtext CHARACTER SET utf8mb4",
				oldNullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Role",
				table: "App",
				nullable: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Role",
				table: "App");

			migrationBuilder.AlterColumn<string>(
				name: "SingleSignOnDestination",
				table: "App",
				type: "longtext CHARACTER SET utf8mb4",
				nullable: true,
				oldClrType: typeof(string));

			migrationBuilder.AlterColumn<string>(
				name: "SingleLogoutResponseDestination",
				table: "App",
				type: "longtext CHARACTER SET utf8mb4",
				nullable: true,
				oldClrType: typeof(string));

			migrationBuilder.AlterColumn<string>(
				name: "Issuer",
				table: "App",
				type: "longtext CHARACTER SET utf8mb4",
				nullable: true,
				oldClrType: typeof(string));
		}
	}
}
