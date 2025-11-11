using System.Collections.Concurrent;
using PSI.Models;

namespace PSI.Services
{
    public class ConcurrentVotingService
    {
        private readonly ConcurrentDictionary<Guid, int> _votes = new();

        public void Upvote(PlaylistSong song)
        {
            _votes.AddOrUpdate(
                song.SongId,
                1, 
                (_, existingVotes) => existingVotes + 1 
            );

            song.Votes = _votes[song.SongId];
        }

        public int GetVotes(Guid songId)
        {
            return _votes.TryGetValue(songId, out var votes) ? votes : 0;
        }
    }
}
