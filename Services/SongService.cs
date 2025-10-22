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
            {
                throw new InvalidOperationException($"Failed to deserialize songs from file:");
            }

            List<Song> songs = MapDtosToEntities(songDtos);
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
