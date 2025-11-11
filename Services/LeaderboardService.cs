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
            var playlists = await _databaseContext.Playlists
                .Where(p => p.IsPublic)
                .OrderByDescending(p => p.Popularity)
                .ThenBy(p => p.Name)
                .Take(topN)
                .Select(p => new PlaylistLeaderboardDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Popularity = p.Popularity,
                    VisitCount = p.VisitCount,
                    IsPublic = p.IsPublic
                })
                .ToListAsync();

            return new LeaderboardResponseDto<PlaylistLeaderboardDto>
            {
                LeaderboardItems = playlists
            };
        }

        public async Task<LeaderboardResponseDto<AlbumLeaderboardDto>> GetAlbumLeaderboardAsync(int topN = 10)
        {
            var albums = await _databaseContext.Albums
                .OrderByDescending(a => a.Popularity)
                .ThenBy(a => a.Name)
                .Take(topN)
                .Select(a => new AlbumLeaderboardDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Artist = a.Artist,
                    ReleaseYear = a.ReleaseYear,
                    Popularity = a.Popularity
                })
                .ToListAsync();

            return new LeaderboardResponseDto<AlbumLeaderboardDto>
            {
                LeaderboardItems = albums
            };
        }
    }
}
