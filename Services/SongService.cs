using PSI.Models;
using Microsoft.EntityFrameworkCore;
using PSI.Data;

namespace PSI.Services
{
    public class SongService
    {
        private readonly AppDbContext _databaseContext;
        private readonly HttpClient _httpClient;

        public SongService(AppDbContext databaseContext, HttpClient httpClient)
        {
            _databaseContext = databaseContext;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await _databaseContext.Songs.ToListAsync();
        }

        public async Task<Song?> GetSongByIdAsync(Guid songId)
        {
            return await _databaseContext.Songs.FindAsync(songId);
        }

        public async Task<(bool Success, string? ErrorMessage, Stream? Stream, string ContentType, string FileName)> GetSongStreamAsync(Guid songId)
        {
            var song = await GetSongByIdAsync(songId);
            if (song == null)
            {
                return (false, "Song not found", null, string.Empty, string.Empty);
            }

            try
            {
                var stream = await song.OpenStreamAsync(_httpClient);
                return (true, null, stream, "audio/mpeg", $"{song.Title}.mp3");
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null, string.Empty, string.Empty);
            }
        }
    }
}
