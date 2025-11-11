using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;
using PSI.Extensions; // ?
using PSI.Exceptions;

namespace PSI.Services
{
    public class PlaylistService
    {
        private readonly AppDbContext _databaseContext;
        private readonly SongService _songService;
        private readonly ConcurrentVotingService _votingService;

        public PlaylistService(AppDbContext databaseContext, SongService songService, ConcurrentVotingService votingService)
        {
            _databaseContext = databaseContext;
            _songService = songService;
            _votingService = votingService;
        }

        public async Task<Playlist> CreatePlaylistAsync(string name, bool isPublic, Guid? currentSongId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Playlist name cannot be empty", nameof(name));

            var playlist = new Playlist(name, isPublic)
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

        public async Task<List<PlaylistSong>> UpvoteSongInPlaylistAsync(Guid playlistId, Guid songId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);

            var playlistSong = playlist.GetSongById(songId);
            if (playlistSong == null)
                throw new KeyNotFoundException("Song not found in playlist");

            _votingService.Upvote(playlistSong);

            await _databaseContext.SaveChangesAsync();
            return playlist.GetAllSongs();
        }

        public async Task<Song> SetCurrentSongAsync(Guid playlistId)
        {
            var playlist = await GetPlaylistByIdAsync(playlistId);
            var playlistSongs = playlist.GetAllSongs();

            // Custom exception used here
            if (!playlistSongs.Any())
                throw new PlaylistOperationException("Cannot set current song because the playlist is empty.");

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
                .Include(p => p.CurrentSong)
                .Include(p => p.Songs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            return playlist ?? throw new KeyNotFoundException("Playlist not found");
        }
    }
}
