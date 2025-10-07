using PSI.Models;
using Microsoft.EntityFrameworkCore;
using PSI.Data;

namespace PSI.Services
{
    public class SongService
    {
        private readonly AppDbContext databaseContext;

        public SongService(AppDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await databaseContext.Songs.ToListAsync();
        }

        public async Task<Song?> GetSongByIdAsync(Guid songId)
        {
            return await databaseContext.Songs.FindAsync(songId);
        }
        //iterating the right way
        //iterates through all songs and prints their display names (nebutina programai, bet ivygdo reikalavima)
        private async Task PrintAllSongDisplayNamesAsync()
        {
            var songs = await GetAllSongsAsync();
            foreach (var song in songs)
            {
                Console.WriteLine(song.GetDisplayName());
            }
        }
    }
}
