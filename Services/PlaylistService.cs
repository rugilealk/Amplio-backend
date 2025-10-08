using PSI.Data;
using PSI.Models;
using Microsoft.EntityFrameworkCore;

namespace PSI.Services
{
    public class PlaylistService
    {
        private readonly AppDbContext _databaseContext;

        public PlaylistService(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Playlist> CreatePlaylistAsync(string name)
        {
            var newPlaylist = new Playlist(name);
            _databaseContext.Playlists.Add(newPlaylist);
            await _databaseContext.SaveChangesAsync();
            return newPlaylist;
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(Guid playlistId)
        {
            return await _databaseContext.Playlists
                .Include(playlist => playlist.Songs)
                .ThenInclude(playlistSong => playlistSong.Song)
                .FirstOrDefaultAsync(playlist => playlist.PlaylistId == playlistId);
        }

        public async Task SaveChangesAsync() => await _databaseContext.SaveChangesAsync();
    }
}
