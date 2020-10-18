using Microsoft.EntityFrameworkCore.Migrations;

namespace FMBot.Persistence.EntityFrameWork.Migrations
{
    public partial class RenameStoredArtistAlbumTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_albums_artists_artist_id",
                table: "albums");

            migrationBuilder.DropForeignKey(
                name: "fk_artist_aliases_artists_artist_id",
                table: "artist_aliases");

            migrationBuilder.DropForeignKey(
                name: "fk_artist_genres_artists_artist_id",
                table: "artist_genres");

            migrationBuilder.DropForeignKey(
                name: "fk_tracks_albums_album_id",
                table: "tracks");

            migrationBuilder.DropForeignKey(
                name: "fk_tracks_artists_artist_id",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "ix_tracks_album_id",
                table: "tracks");

            migrationBuilder.DropColumn(
                name: "album_id",
                table: "tracks");

            migrationBuilder.AddColumn<int>(
                name: "cached_album_id",
                table: "tracks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tracks_cached_album_id",
                table: "tracks",
                column: "cached_album_id");

            migrationBuilder.AddForeignKey(
                name: "fk_albums_artists_cached_artist_id",
                table: "albums",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_artist_aliases_artists_cached_artist_id",
                table: "artist_aliases",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_artist_genres_artists_cached_artist_id",
                table: "artist_genres",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tracks_artists_cached_artist_id",
                table: "tracks",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tracks_albums_cached_album_id",
                table: "tracks",
                column: "cached_album_id",
                principalTable: "albums",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_albums_artists_cached_artist_id",
                table: "albums");

            migrationBuilder.DropForeignKey(
                name: "fk_artist_aliases_artists_cached_artist_id",
                table: "artist_aliases");

            migrationBuilder.DropForeignKey(
                name: "fk_artist_genres_artists_cached_artist_id",
                table: "artist_genres");

            migrationBuilder.DropForeignKey(
                name: "fk_tracks_artists_cached_artist_id",
                table: "tracks");

            migrationBuilder.DropForeignKey(
                name: "fk_tracks_albums_cached_album_id",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "ix_tracks_cached_album_id",
                table: "tracks");

            migrationBuilder.DropColumn(
                name: "cached_album_id",
                table: "tracks");

            migrationBuilder.AddColumn<int>(
                name: "album_id",
                table: "tracks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tracks_album_id",
                table: "tracks",
                column: "album_id");

            migrationBuilder.AddForeignKey(
                name: "fk_albums_artists_artist_id",
                table: "albums",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_artist_aliases_artists_artist_id",
                table: "artist_aliases",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_artist_genres_artists_artist_id",
                table: "artist_genres",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tracks_albums_album_id",
                table: "tracks",
                column: "album_id",
                principalTable: "albums",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tracks_artists_artist_id",
                table: "tracks",
                column: "artist_id",
                principalTable: "artists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
