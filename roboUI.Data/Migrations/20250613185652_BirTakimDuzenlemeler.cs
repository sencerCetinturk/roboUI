using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace roboUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class BirTakimDuzenlemeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemsChoiceSelections_OptionChoices_OptionChoideId",
                table: "OrderItemsChoiceSelections");

            migrationBuilder.RenameColumn(
                name: "OptionChoideId",
                table: "OrderItemsChoiceSelections",
                newName: "OptionChoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItemsChoiceSelections_OptionChoideId",
                table: "OrderItemsChoiceSelections",
                newName: "IX_OrderItemsChoiceSelections_OptionChoiceId");

            migrationBuilder.RenameColumn(
                name: "IsRequiredForProduct",
                table: "OptionGroups",
                newName: "IsRequired");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CoffeeProducts",
                newName: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemsChoiceSelections_OptionChoices_OptionChoiceId",
                table: "OrderItemsChoiceSelections",
                column: "OptionChoiceId",
                principalTable: "OptionChoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemsChoiceSelections_OptionChoices_OptionChoiceId",
                table: "OrderItemsChoiceSelections");

            migrationBuilder.RenameColumn(
                name: "OptionChoiceId",
                table: "OrderItemsChoiceSelections",
                newName: "OptionChoideId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItemsChoiceSelections_OptionChoiceId",
                table: "OrderItemsChoiceSelections",
                newName: "IX_OrderItemsChoiceSelections_OptionChoideId");

            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "OptionGroups",
                newName: "IsRequiredForProduct");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CoffeeProducts",
                newName: "Title");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemsChoiceSelections_OptionChoices_OptionChoideId",
                table: "OrderItemsChoiceSelections",
                column: "OptionChoideId",
                principalTable: "OptionChoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
