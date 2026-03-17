using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G360.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PizzaTypeIdInPizzas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pizzas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Pizzas",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}
