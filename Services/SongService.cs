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
    }
}
