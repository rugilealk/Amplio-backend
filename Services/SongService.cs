using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;
using System.Text.RegularExpressions;

namespace PSI.Services
{
    public class SongService
    {
        private readonly AppDbContext _databaseContext;

        public SongService(AppDbContext databaseContext, HttpClient httpClient)
        {
            _databaseContext = databaseContext;
        }

        public string ConvertDriveLink(string link)
        {
            var match = Regex.Match(link, @"\/d\/([a-zA-Z0-9_-]+)\/");
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

    }
}
