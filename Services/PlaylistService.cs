using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;

namespace PSI.Services
{
    public class PlaylistService
    {
        private readonly AppDbContext _databaseContext;
        private readonly SongService _songService;

        public PlaylistService(AppDbContext databaseContext, SongService songService)
        {
            _databaseContext = databaseContext;
            _songService = songService;
        }

        public async Task<Playlist> CreatePlaylistAsync(string name)
        {
            var newPlaylist = new Playlist(name);
            _databaseContext.Playlists.Add(newPlaylist);
            await _databaseContext.SaveChangesAsync();
            return newPlaylist;
        }

        public async Task<List<PlaylistSong>?> GetSongsInPlaylistAsync(Guid playlistId)
        {
            var playlist = await _databaseContext.Playlists
                .Include(p => p.Songs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            return playlist?.GetAllSongs();
        }

        public async Task<(bool Success, string? ErrorMessage, List<PlaylistSong>? Songs)> AddSongToPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return (false, "Playlist not found", null);

            var song = await _songService.GetSongByIdAsync(songId);
            if (song == null) return (false, "Song not found", null);

            playlist.AddSong(song);
            await _databaseContext.SaveChangesAsync();
            return (true, null, playlist.GetAllSongs());
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return (false, "Playlist not found");

            var wasDeleted = playlist.DeleteSong(songId);
            if (!wasDeleted) return (false, "Song not found in playlist");

            await _databaseContext.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<PlaylistSong>? Songs)> UpvoteSongInPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            if (playlist == null) return (false, "Playlist not found", null);

            playlist.UpvoteSong(songId);
            await _databaseContext.SaveChangesAsync();
            return (true, null, playlist.GetAllSongs());
        }

        private async Task<Playlist?> GetPlaylistByIdAsync(Guid playlistId)
        {
            return await _databaseContext.Playlists
                .Include(p => p.Songs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
        }
    }
}
