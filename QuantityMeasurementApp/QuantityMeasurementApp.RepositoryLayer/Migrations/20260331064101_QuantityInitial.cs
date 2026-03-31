using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuantityMeasurementApp.RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class QuantityInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuantityHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OperationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstValue = table.Column<double>(type: "double precision", nullable: false),
                    FirstUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondValue = table.Column<double>(type: "double precision", nullable: true),
                    SecondUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TargetUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultValue = table.Column<double>(type: "double precision", nullable: false),
                    ResultUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuantityHistory");
        }
    }
}
