using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStoreFaker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BandWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandWords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FirstNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LastNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SongTitleWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WordType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongTitleWords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTitles_Locale",
                table: "AlbumTitles",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_BandWords_Locale",
                table: "BandWords",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_FirstNames_Locale_Gender",
                table: "FirstNames",
                columns: new[] { "Locale", "Gender" });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Locale",
                table: "Genres",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_LastNames_Locale",
                table: "LastNames",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Locales_Code",
                table: "Locales",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SongTitleWords_Locale_WordType",
                table: "SongTitleWords",
                columns: new[] { "Locale", "WordType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumTitles");

            migrationBuilder.DropTable(
                name: "BandWords");

            migrationBuilder.DropTable(
                name: "FirstNames");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "LastNames");

            migrationBuilder.DropTable(
                name: "Locales");

            migrationBuilder.DropTable(
                name: "SongTitleWords");
        }
    }
}
