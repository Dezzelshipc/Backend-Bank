using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganisationId",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationId",
                table: "Branches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_OrganisationId_Name",
                table: "Services",
                columns: new[] { "OrganisationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_Login",
                table: "Organisations",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_OrganisationId_Name",
                table: "Branches",
                columns: new[] { "OrganisationId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Services_OrganisationId_Name",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_Login",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Branches_OrganisationId_Name",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Branches");
        }
    }
}
