using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Added_More_Info_for_Record : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Genre",
                table: "Records",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReissue",
                table: "Records",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordLabel",
                table: "Records",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "IsReissue",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "RecordLabel",
                table: "Records");
        }
    }
}
