using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookWebShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProductISBNAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "Products");
        }
    }
}
