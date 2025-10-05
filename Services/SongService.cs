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

        // mums irgi sito nereik gal tada? 
        public async Task<Song> CreateSongAsync(string songTitle, string songArtist, List<Genre> genres, string path)
        {
            var newSong = new Song(songTitle, songArtist, genres, path);
            databaseContext.Songs.Add(newSong);
            await databaseContext.SaveChangesAsync();
            return newSong;
        }
    }
}
