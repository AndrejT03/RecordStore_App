using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Added_New_Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artist_Country_CountryId",
                table: "Artist");

            migrationBuilder.DropForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordInShoppingCart_Record_RecordId",
                table: "RecordInShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordInShoppingCart_ShoppingCart_ShoppingCartId",
                table: "RecordInShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_OwnerId",
                table: "ShoppingCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCart",
                table: "ShoppingCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecordInShoppingCart",
                table: "RecordInShoppingCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Record",
                table: "Record");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Country",
                table: "Country");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artist",
                table: "Artist");

            migrationBuilder.RenameTable(
                name: "ShoppingCart",
                newName: "ShoppingCarts");

            migrationBuilder.RenameTable(
                name: "RecordInShoppingCart",
                newName: "RecordInShoppingCarts");

            migrationBuilder.RenameTable(
                name: "Record",
                newName: "Records");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "Artist",
                newName: "Artists");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCart_OwnerId",
                table: "ShoppingCarts",
                newName: "IX_ShoppingCarts_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_RecordInShoppingCart_ShoppingCartId",
                table: "RecordInShoppingCarts",
                newName: "IX_RecordInShoppingCarts_ShoppingCartId");

            migrationBuilder.RenameIndex(
                name: "IX_RecordInShoppingCart_RecordId",
                table: "RecordInShoppingCarts",
                newName: "IX_RecordInShoppingCarts_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Record_ArtistId",
                table: "Records",
                newName: "IX_Records_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_Artist_CountryId",
                table: "Artists",
                newName: "IX_Artists_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCarts",
                table: "ShoppingCarts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecordInShoppingCarts",
                table: "RecordInShoppingCarts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Records",
                table: "Records",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artists",
                table: "Artists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Countries_CountryId",
                table: "Artists",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordInShoppingCarts_Records_RecordId",
                table: "RecordInShoppingCarts",
                column: "RecordId",
                principalTable: "Records",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordInShoppingCarts_ShoppingCarts_ShoppingCartId",
                table: "RecordInShoppingCarts",
                column: "ShoppingCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Artists_ArtistId",
                table: "Records",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_OwnerId",
                table: "ShoppingCarts",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Countries_CountryId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordInShoppingCarts_Records_RecordId",
                table: "RecordInShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordInShoppingCarts_ShoppingCarts_ShoppingCartId",
                table: "RecordInShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Artists_ArtistId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_OwnerId",
                table: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCarts",
                table: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Records",
                table: "Records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecordInShoppingCarts",
                table: "RecordInShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artists",
                table: "Artists");

            migrationBuilder.RenameTable(
                name: "ShoppingCarts",
                newName: "ShoppingCart");

            migrationBuilder.RenameTable(
                name: "Records",
                newName: "Record");

            migrationBuilder.RenameTable(
                name: "RecordInShoppingCarts",
                newName: "RecordInShoppingCart");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Country");

            migrationBuilder.RenameTable(
                name: "Artists",
                newName: "Artist");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCarts_OwnerId",
                table: "ShoppingCart",
                newName: "IX_ShoppingCart_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Records_ArtistId",
                table: "Record",
                newName: "IX_Record_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_RecordInShoppingCarts_ShoppingCartId",
                table: "RecordInShoppingCart",
                newName: "IX_RecordInShoppingCart_ShoppingCartId");

            migrationBuilder.RenameIndex(
                name: "IX_RecordInShoppingCarts_RecordId",
                table: "RecordInShoppingCart",
                newName: "IX_RecordInShoppingCart_RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists_CountryId",
                table: "Artist",
                newName: "IX_Artist_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCart",
                table: "ShoppingCart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Record",
                table: "Record",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecordInShoppingCart",
                table: "RecordInShoppingCart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Country",
                table: "Country",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artist",
                table: "Artist",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Artist_Country_CountryId",
                table: "Artist",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Record_Artist_ArtistId",
                table: "Record",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordInShoppingCart_Record_RecordId",
                table: "RecordInShoppingCart",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordInShoppingCart_ShoppingCart_ShoppingCartId",
                table: "RecordInShoppingCart",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_OwnerId",
                table: "ShoppingCart",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
