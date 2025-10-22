using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;
using PSI.Extensions;

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

        public async Task<Playlist> CreatePlaylistAsync(string name, Guid? currentSongId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Playlist name cannot be empty", nameof(name));

            var playlist = new Playlist(name)
            {
                CurrentSongId = currentSongId
            };

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

        //extension method usage
        public async Task<List<PlaylistSong>> UpvoteSongInPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            if (!playlist.UpvoteSongById(songId))
                throw new KeyNotFoundException("Song not found in playlist");

            await _databaseContext.SaveChangesAsync();
            return playlist.GetAllSongs();
        }


        public async Task<Song> SetCurrentSongAsync(Guid playlistId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            var playlistSongs = playlist.GetAllSongs();

            if (!playlistSongs.Any())
            {
                throw new InvalidOperationException("No songs available in the playlist to set as current.");
            }

            var topSong = playlistSongs.First().Song;
            playlist.CurrentSong = topSong;

            playlist.DeleteSong(topSong.Id);

            await _databaseContext.SaveChangesAsync();
            return playlist.CurrentSong;
        }

        public async Task<Song?> GetCurrentSongAsync(Guid playlistId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            return playlist.CurrentSong;
        }

        private async Task<Playlist> GetPlaylistByIdAsync(Guid playlistId)
        {
            var playlist = await _databaseContext.Playlists
                .Include(playlist => playlist.CurrentSong)
                .Include(playlist => playlist.Songs)
                .ThenInclude(playlistSong => playlistSong.Song)
                .FirstOrDefaultAsync(playlist => playlist.PlaylistId == playlistId);

            return playlist ?? throw new KeyNotFoundException("Playlist not found");
        }
    }
}
