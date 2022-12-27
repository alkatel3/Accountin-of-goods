using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priсe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GoodsStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_Goods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false),
                    UserStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_UserId", x => x.Id);
                    table.UniqueConstraint("AK_Users_PhoneNumber", x => x.PhoneNumber);
                });

            migrationBuilder.CreateTable(
                name: "GoodsInStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_GoodsInStockId", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_GoodsInStock_Goods",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueueForPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_QueueForPurchaseId", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_QueueForPurchase_Goods",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_OrderListId", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_OrderList_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    OrderListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_OrderId", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Order_Goods",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Order_OrderList",
                        column: x => x.OrderListId,
                        principalTable: "OrderLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsInStock_GoodsId",
                table: "GoodsInStock",
                column: "GoodsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLists_UserId",
                table: "OrderLists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GoodsId",
                table: "Orders",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderListId",
                table: "Orders",
                column: "OrderListId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueForPurchases_GoodsId",
                table: "QueueForPurchases",
                column: "GoodsId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsInStock");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "QueueForPurchases");

            migrationBuilder.DropTable(
                name: "OrderLists");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
