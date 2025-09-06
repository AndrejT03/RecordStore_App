using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Added_Record_Label : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordLabel",
                table: "Records");

            migrationBuilder.AddColumn<Guid>(
                name: "RecordLabelId",
                table: "Records",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RecordLabels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordLabels_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_RecordLabelId",
                table: "Records",
                column: "RecordLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLabels_CountryId",
                table: "RecordLabels",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_RecordLabels_RecordLabelId",
                table: "Records",
                column: "RecordLabelId",
                principalTable: "RecordLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_RecordLabels_RecordLabelId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "RecordLabels");

            migrationBuilder.DropIndex(
                name: "IX_Records_RecordLabelId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "RecordLabelId",
                table: "Records");

            migrationBuilder.AddColumn<string>(
                name: "RecordLabel",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
