using FluentAssertions;
using PSI.Models;

namespace Tests.Unit;

public class GenericLeaderboardTests
{
    [Fact]
    public void AddSongCollection_AddsUnique()
    {
        var leaderboard = new GenericLeaderboard<Album>();
        var a1 = new Album("A","B",2000);
        leaderboard.AddSongCollection(a1);
        leaderboard.AddSongCollection(a1);
        leaderboard.LeaderboardItems.Should().HaveCount(1);
    }

    [Fact]
    public void GetSortedByPopularity_SortsCorrectly()
    {
        var leaderboard = new GenericLeaderboard<Album>();
        var a1 = new Album("A","B",2000) { Popularity = 5 };
        var a2 = new Album("B","B",2000) { Popularity = 10 };
        leaderboard.AddSongCollection(a1);
        leaderboard.AddSongCollection(a2);
        var sorted = leaderboard.GetSortedByPopularity();
        sorted.First().Popularity.Should().Be(10);
    }
}
