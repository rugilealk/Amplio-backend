using FluentAssertions;
using PSI.Models;
using PSI.Services;

namespace Tests.Unit;

public class ConcurrentVotingServiceTests
{
    [Fact]
    public void Upvote_IncrementsVotes()
    {
        var voting = new ConcurrentVotingService();
        var song = new Song { Id = Guid.NewGuid(), Title = "T" };
        var playlist = new Playlist("P", true);
        playlist.AddSong(song);
        var playlistSong = playlist.Songs.First();
        voting.Upvote(playlistSong);
        voting.Upvote(playlistSong);
        playlistSong.Votes.Should().Be(2);
        voting.GetVotes(song.Id).Should().Be(2);
    }
}
