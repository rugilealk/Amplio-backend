using PSI.Models;

namespace PSI.Services.Interfaces
{
    public interface IConcurrentVotingService
    {
        void Upvote(PlaylistSong song);
        int GetVotes(Guid songId);
    }
}
