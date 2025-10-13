using PSI.Models;
using Microsoft.EntityFrameworkCore;
using PSI.Data;

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

        public async Task<(bool Success, string? ErrorMessage, Stream? Stream, string ContentType, string FileName)> GetSongStreamAsync(Guid songId)
        {
            var song = await GetSongByIdAsync(songId);
            if (song == null)
            {
                return (false, "Song not found", null, string.Empty, string.Empty);
            }

            var fullPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", song.FilePath.Value);
            fullPath = Path.GetFullPath(fullPath);

            if (!System.IO.File.Exists(fullPath))
            {
                return (false, $"File not found: {fullPath}", null, string.Empty, string.Empty);
            }

            var fileStream = song.OpenStream();
            var contentType = "audio/mpeg";
            var fileName = Path.GetFileName(fullPath);

            return (true, null, fileStream, contentType, fileName);
        }
    }
}
