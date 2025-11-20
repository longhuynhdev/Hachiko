using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hachiko.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleAddressesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StreetAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CityProvince = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.CheckConstraint("CK_Address_Owner", "(ApplicationUserId IS NOT NULL AND CompanyId IS NULL) OR (ApplicationUserId IS NULL AND CompanyId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Addresses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId",
                table: "Addresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CompanyId",
                table: "Addresses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_IsDefault",
                table: "Addresses",
                column: "IsDefault");

            // Migrate existing user addresses to the new Addresses table
            migrationBuilder.Sql(@"
                INSERT INTO Addresses (StreetAddress, Ward, District, CityProvince, PostalCode, IsDefault, Label, ApplicationUserId, CreatedAt)
                SELECT
                    ISNULL(StreetAddress, ''),
                    ISNULL(State, 'N/A'),
                    'N/A',
                    ISNULL(City, 'N/A'),
                    PostalCode,
                    1,
                    'Default',
                    Id,
                    GETDATE()
                FROM Users
                WHERE StreetAddress IS NOT NULL OR City IS NOT NULL OR State IS NOT NULL OR PostalCode IS NOT NULL
            ");

            // Migrate existing company addresses to the new Addresses table
            migrationBuilder.Sql(@"
                INSERT INTO Addresses (StreetAddress, Ward, District, CityProvince, PostalCode, IsDefault, Label, CompanyId, CreatedAt)
                SELECT
                    ISNULL(StreetAddress, ''),
                    ISNULL(State, 'N/A'),
                    'N/A',
                    ISNULL(City, 'N/A'),
                    PostalCode,
                    1,
                    'Office',
                    Id,
                    GETDATE()
                FROM Companies
                WHERE StreetAddress IS NOT NULL OR City IS NOT NULL OR State IS NOT NULL OR PostalCode IS NOT NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
