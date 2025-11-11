using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.DTOs;
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
        public async Task<LeaderboardResponseDto<PlaylistLeaderboardDto>> GetPlaylistLeaderboardAsync(int topN = 10)
        {
            var leaderboard = new GenericLeaderboard<Playlist>();

            var playlists = await _databaseContext.Playlists
                .AsNoTracking()
                .Where(p => p.IsPublic)
                .ToListAsync();

            foreach (var playlist in playlists)
            {
                leaderboard.AddSongCollection(playlist);
            }

            var sortedPlaylists = leaderboard.GetSortedByPopularity()
                .Take(topN)
                .Select(p => new PlaylistLeaderboardDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Popularity = p.Popularity,
                    VisitCount = p.VisitCount,
                    IsPublic = p.IsPublic
                })
                .ToList();

            return new LeaderboardResponseDto<PlaylistLeaderboardDto>
            {
                LeaderboardItems = sortedPlaylists
            };
        }

        public async Task<LeaderboardResponseDto<AlbumLeaderboardDto>> GetAlbumLeaderboardAsync(int topN = 10)
        {
            var leaderboard = new GenericLeaderboard<Album>();

            var albums = await _databaseContext.Albums
                .AsNoTracking()
                .ToListAsync();

            foreach (var album in albums)
            {
                leaderboard.AddSongCollection(album);
            }

            var sortedAlbums = leaderboard.GetSortedByPopularity()
                .Take(topN)
                .Select(a => new AlbumLeaderboardDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Artist = a.Artist,
                    ReleaseYear = a.ReleaseYear,
                    Popularity = a.Popularity
                })
                .ToList();

            return new LeaderboardResponseDto<AlbumLeaderboardDto>
            {
                LeaderboardItems = sortedAlbums
            };
        }
    }
}
