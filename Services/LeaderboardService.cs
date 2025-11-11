using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;

namespace PSI.Services
{
    public class LeaderboardService
    {
        private readonly AppDbContext _databaseContext;

        public LeaderboardService(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<GenericLeaderboard<Playlist>> GetPlaylistLeaderboardAsync(int topN = 10)
        {
            var leaderboard = new GenericLeaderboard<Playlist>();

            var playlists = await _databaseContext.Playlists
                .Where(p => p.IsPublic)
                .Include(p => p.Songs)
                .ToListAsync();

            foreach (var playlist in playlists)
            {
                leaderboard.AddSongCollection(playlist);
            }

            var sorted = leaderboard.GetSortedByPopularity(topN);

            return new GenericLeaderboard<Playlist> { LeaderboardItems = sorted };
        }
        public async Task<GenericLeaderboard<Album>> GetAlbumLeaderboardAsync(int topN = 10)
        {
            var leaderboard = new GenericLeaderboard<Album>();

            var albums = await _databaseContext.Albums
                .Include(a => a.Songs)
                .ToListAsync();

            foreach (var album in albums)
            {
                leaderboard.AddSongCollection(album);
            }
            return leaderboard;
        }
    }
}
