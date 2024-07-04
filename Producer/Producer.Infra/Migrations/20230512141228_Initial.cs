using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Producer.Infra.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Persons",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					FirstName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
					LastName = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
					Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Persons", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Persons");
		}
	}
}
