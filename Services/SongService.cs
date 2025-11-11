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
            await ClearExistingDataAsync();

            List<SongDto>? songDtos = await LoadSongsFromFileAsync();
            if (songDtos == null)
                throw new InvalidOperationException("Failed to deserialize songs from file");

            var songs = new List<Song>();
            var albums = new Dictionary<string, Album>(); // Track albums by key

            foreach (var dto in songDtos)
            {
                Album? albumEntity = null;

                if (dto.Album != null)
                {
                    string albumKey = $"{dto.Album.Name}|{dto.Album.Artist}";

                    // Check in-memory dictionary first
                    if (!albums.TryGetValue(albumKey, out albumEntity))
                    {
                        // Check database
                        albumEntity = await _databaseContext.Albums
                            .FirstOrDefaultAsync(a => a.Name == dto.Album.Name && a.Artist == dto.Album.Artist);

                        if (albumEntity == null)
                        {
                            albumEntity = new Album(dto.Album.Name, dto.Album.Artist, dto.Album.ReleaseYear);
                            _databaseContext.Albums.Add(albumEntity);
                        }

                        albums[albumKey] = albumEntity;
                    }
                }

                var song = new Song
                {
                    Title = dto.Title,
                    Artist = dto.Artist,
                    Link = new SongLink(dto.Link),
                    Genres = dto.Genres.ToList(),
                    Album = albumEntity,
                    AlbumId = albumEntity?.Id
                };

                songs.Add(song);
            }

            // Save albums first
            await _databaseContext.SaveChangesAsync();

            // Then save all songs at once
            _databaseContext.Songs.AddRange(songs);
            await _databaseContext.SaveChangesAsync();

            return songs;
        }

        private async Task ClearExistingDataAsync()
        {
            var playlists = await _databaseContext.Playlists.ToListAsync();
            playlists.ForEach(p => p.CurrentSongId = null);
            await _databaseContext.SaveChangesAsync();

            var playlistSongs = await _databaseContext.PlaylistSongs.ToListAsync();
            _databaseContext.PlaylistSongs.RemoveRange(playlistSongs);

            var songs = await _databaseContext.Songs.ToListAsync();
            _databaseContext.Songs.RemoveRange(songs);

            var albums = await _databaseContext.Albums.ToListAsync();
            _databaseContext.Albums.RemoveRange(albums);

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
