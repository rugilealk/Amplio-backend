using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.DTOs;
using PSI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PSI.Services
{
    public class SongService
    {
        private readonly AppDbContext _databaseContext;

        public SongService(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await _databaseContext.Songs.ToListAsync();
        }

        public async Task<Song?> GetSongByIdAsync(Guid songId)
        {
            return await _databaseContext.Songs.FindAsync(songId);
        }

        public async Task<List<Song>> ImportSongsFromFileAsync()
        {
            // 1️⃣ Clear existing data
            await ClearExistingDataAsync();

            // 2️⃣ Load JSON file
            List<SongDto>? songDtos = await LoadSongsFromFileAsync();
            if (songDtos == null)
                throw new InvalidOperationException("Failed to deserialize songs from file");

            var songs = new List<Song>();

            foreach (var dto in songDtos)
            {
                Album? albumEntity = null;

                // 3️⃣ Handle album if provided
                if (dto.Album != null)
                {
                    // Check if album already exists in DB
                    albumEntity = await _databaseContext.Albums
                        .FirstOrDefaultAsync(a => a.Name == dto.Album.Name && a.Artist == dto.Album.Artist);

                    // Create album if it doesn't exist
                    if (albumEntity == null)
                    {
                        albumEntity = new Album(dto.Album.Name, dto.Album.Artist, dto.Album.ReleaseYear);
                        _databaseContext.Albums.Add(albumEntity);
                        await _databaseContext.SaveChangesAsync(); // ensures Album.Id is valid
                    }
                }

                // 4️⃣ Create song with correct AlbumId
                var song = new Song
                {
                    Title = dto.Title,
                    Artist = dto.Artist,
                    Link = new SongLink(dto.Link),
                    Genres = dto.Genres.ToList(),
                    Album = albumEntity,
                    AlbumId = albumEntity?.Id
                };

                // 5️⃣ Optional: add song to album's in-memory Songs list
                albumEntity?.AddSong(song);

                songs.Add(song);
            }

            // 6️⃣ Save all songs to DB
            _databaseContext.Songs.AddRange(songs);
            await _databaseContext.SaveChangesAsync();

            return songs;
        }

        private async Task ClearExistingDataAsync()
        {
            var playlists = await _databaseContext.Playlists.ToListAsync();
            playlists.ForEach(p => p.CurrentSongId = null);

            var playlistSongs = _databaseContext.PlaylistSongs.ToList();
            _databaseContext.PlaylistSongs.RemoveRange(playlistSongs);

            var existingSongs = _databaseContext.Songs.ToList();
            _databaseContext.Songs.RemoveRange(existingSongs);

            var existingAlbums = _databaseContext.Albums.ToList();
            _databaseContext.Albums.RemoveRange(existingAlbums);

            await _databaseContext.SaveChangesAsync();
        }

        private async Task<List<SongDto>?> LoadSongsFromFileAsync()
        {
            string filePath = Path.Combine(
                path1: Directory.GetCurrentDirectory(),
                path2: "DummyData",
                path3: "songs.json"
            );

            using Stream fileStream = File.OpenRead(filePath);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            var songDtos = await JsonSerializer.DeserializeAsync<List<SongDto>>(fileStream, options);
            return songDtos;
        }
        private List<Song> MapDtosToEntities(List<SongDto> songDtos)
        {
            var songs = new List<Song>();
            foreach (SongDto dto in songDtos)
            {
                var song = new Song
                {
                    Title = dto.Title,
                    Artist = dto.Artist,
                    Link = new SongLink(dto.Link),
                    Genres = dto.Genres.ToList()
                };
                songs.Add(song);
            }
            return songs;
        }
    }
}
