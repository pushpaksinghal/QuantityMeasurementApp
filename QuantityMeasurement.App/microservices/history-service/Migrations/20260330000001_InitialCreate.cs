using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HistoryService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuantityHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstValue = table.Column<double>(type: "float", nullable: false),
                    FirstUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SecondValue = table.Column<double>(type: "float", nullable: true),
                    SecondUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TargetUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResultValue = table.Column<double>(type: "float", nullable: false),
                    ResultUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false,
                        defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuantityHistory_Category",
                table: "QuantityHistory",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityHistory_CreatedAt",
                table: "QuantityHistory",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QuantityHistory");
        }
    }
}
