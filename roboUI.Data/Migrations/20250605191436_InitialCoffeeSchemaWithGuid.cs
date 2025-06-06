using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace roboUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCoffeeSchemaWithGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoffeeCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SelectionType = table.Column<string>(type: "TEXT", nullable: false),
                    IsRequiredForProduct = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeProducts_CoffeeCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CoffeeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OptionChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OptionGroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AdditionalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxQuantityAllowed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionChoices_OptionGroups_OptionGroupId",
                        column: x => x.OptionGroupId,
                        principalTable: "OptionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoffeeProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CalculatedUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_CoffeeProducts_CoffeeProductId",
                        column: x => x.CoffeeProductId,
                        principalTable: "CoffeeProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionChoicesDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoffeeProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OptionGroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefaultOptionChoiceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsRequiredOverride = table.Column<bool>(type: "INTEGER", nullable: true),
                    DisplayOrderOverride = table.Column<int>(type: "INTEGER", nullable: true),
                    OptionChoiceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionChoicesDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionChoicesDefinitions_CoffeeProducts_CoffeeProductId",
                        column: x => x.CoffeeProductId,
                        principalTable: "CoffeeProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionChoicesDefinitions_OptionChoices_DefaultOptionChoiceId",
                        column: x => x.DefaultOptionChoiceId,
                        principalTable: "OptionChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OptionChoicesDefinitions_OptionChoices_OptionChoiceId",
                        column: x => x.OptionChoiceId,
                        principalTable: "OptionChoices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OptionChoicesDefinitions_OptionGroups_OptionGroupId",
                        column: x => x.OptionGroupId,
                        principalTable: "OptionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemsChoiceSelections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OptionChoideId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SelectedQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceAtSelection = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemsChoiceSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemsChoiceSelections_OptionChoices_OptionChoideId",
                        column: x => x.OptionChoideId,
                        principalTable: "OptionChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItemsChoiceSelections_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeProducts_CategoryId",
                table: "CoffeeProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionChoices_OptionGroupId",
                table: "OptionChoices",
                column: "OptionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionChoicesDefinitions_CoffeeProductId",
                table: "OptionChoicesDefinitions",
                column: "CoffeeProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionChoicesDefinitions_DefaultOptionChoiceId",
                table: "OptionChoicesDefinitions",
                column: "DefaultOptionChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionChoicesDefinitions_OptionChoiceId",
                table: "OptionChoicesDefinitions",
                column: "OptionChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionChoicesDefinitions_OptionGroupId",
                table: "OptionChoicesDefinitions",
                column: "OptionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CoffeeProductId",
                table: "OrderItems",
                column: "CoffeeProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsChoiceSelections_OptionChoideId",
                table: "OrderItemsChoiceSelections",
                column: "OptionChoideId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsChoiceSelections_OrderItemId",
                table: "OrderItemsChoiceSelections",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionChoicesDefinitions");

            migrationBuilder.DropTable(
                name: "OrderItemsChoiceSelections");

            migrationBuilder.DropTable(
                name: "OptionChoices");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OptionGroups");

            migrationBuilder.DropTable(
                name: "CoffeeProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CoffeeCategories");
        }
    }
}
