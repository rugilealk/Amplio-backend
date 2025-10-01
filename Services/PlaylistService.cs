using PSI.Data;
using PSI.Models;
using Microsoft.EntityFrameworkCore;

namespace PSI.Services
{
    public class PlaylistService
    {
        private readonly AppDbContext databaseContext;

        public PlaylistService(AppDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Playlist> CreatePlaylistAsync()
        {
            var newPlaylist = new Playlist();
            databaseContext.Playlists.Add(newPlaylist);
            await databaseContext.SaveChangesAsync();
            return newPlaylist;
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(Guid playlistId)
        {
            return await databaseContext.Playlists
                .Include(playlist => playlist.Songs)
                .ThenInclude(playlistSong => playlistSong.Song)
                .FirstOrDefaultAsync(playlist => playlist.PlaylistId == playlistId);
        }

        public async Task SaveChangesAsync() => await databaseContext.SaveChangesAsync();
    }
}
