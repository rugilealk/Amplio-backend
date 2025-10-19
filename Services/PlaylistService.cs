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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Playlist name cannot be empty", nameof(name));

            var playlist = new Playlist(name);
            _databaseContext.Playlists.Add(playlist);
            await _databaseContext.SaveChangesAsync();
            return playlist;
        }

        public async Task<List<PlaylistSong>> GetSongsInPlaylistAsync(Guid playlistId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            return playlist.GetAllSongs();
        }

        public async Task<List<PlaylistSong>> AddSongToPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            var song = await _songService.GetSongByIdAsync(songId)
                ?? throw new KeyNotFoundException("Song not found");

            playlist.AddSong(song);
            await _databaseContext.SaveChangesAsync();
            return playlist.GetAllSongs();
        }

        public async Task RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            if (!playlist.DeleteSong(songId))
            {
                throw new KeyNotFoundException("Song not found in playlist");
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<PlaylistSong>> UpvoteSongInPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            var song = playlist.GetSongById(songId)
                ?? throw new KeyNotFoundException("Song not found in playlist");

            song.Upvote();
            await _databaseContext.SaveChangesAsync();
            return playlist.GetAllSongs();
        }

        public async Task<Song> SetCurrentSongAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            var playlistSong = playlist.GetSongById(songId)
                ?? throw new KeyNotFoundException("Song not found in playlist");

            playlist.CurrentSong = playlistSong.Song;

            playlist.DeleteSong(songId);

            await _databaseContext.SaveChangesAsync();
            return playlist.CurrentSong;
        }

        private async Task<Playlist> GetPlaylistByIdAsync(Guid playlistId)
        {
            var playlist = await _databaseContext.Playlists
                .Include(p => p.Songs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            return playlist ?? throw new KeyNotFoundException("Playlist not found");
        }
    }
}
