using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.DTOs;
using PSI.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PSI.Services
{
    public class SongService
    {
        private readonly AppDbContext _databaseContext;

        public SongService(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public string ConvertDriveLink(string link)
        {
            Match match = Regex.Match(link, @"\/d\/([a-zA-Z0-9_-]+)\/");
            if (!match.Success)
            {
                throw new InvalidOperationException("Invalid Google Drive link format");
            }

            var fileId = match.Groups[1].Value;
            return $"https://drive.google.com/uc?export=download&id={fileId}";
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
            List<Song> existingSongs = _databaseContext.Songs.ToList();
            if (existingSongs.Any())
            {
                _databaseContext.Songs.RemoveRange(existingSongs);
                await _databaseContext.SaveChangesAsync();
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "DummyData", "songs.json");
            using Stream fileStream = File.OpenRead(filePath);
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

            List<SongDto>? songDtos = await JsonSerializer.DeserializeAsync<List<SongDto>>(fileStream, options);

            if (songDtos == null) return new List<Song>();

            List<Song> songs = new List<Song>();
            foreach (SongDto dto in songDtos)
            {
                var song = new Song
                {
                    Title = dto.Title,
                    Artist = dto.Artist,
                    Link = dto.Link,
                    Genres = dto.Genres.ToList()
                };
                songs.Add(song);
                _databaseContext.Songs.Add(song);
            }

            await _databaseContext.SaveChangesAsync();
            return songs;
        }
    }
}
