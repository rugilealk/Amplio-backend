using PSI.Models;
using PSI.Services.Interfaces;
using PSI.Repositories.Interfaces;

namespace PSI.Services
{
    public class ConcurrentVotingService : IConcurrentVotingService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public ConcurrentVotingService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task UpvoteAsync(Guid playlistId, Guid songId)
        {
            await _semaphore.WaitAsync();
            try
            {
                var playlist = await _playlistRepository.GetDetailedByIdAsync(playlistId);
                if (playlist == null)
                    throw new KeyNotFoundException("Playlist not found");

                var playlistSong = playlist.GetSongById(songId);
                if (playlistSong == null)
                    throw new KeyNotFoundException("Song not found in playlist");

                playlistSong.Upvote();
                await _playlistRepository.UpdateAsync(playlist);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<int> GetVotesAsync(Guid playlistId, Guid songId)
        {
            var playlist = await _playlistRepository.GetDetailedByIdAsync(playlistId);
            var playlistSong = playlist?.GetSongById(songId);
            return playlistSong?.Votes ?? 0;
        }
    }
}