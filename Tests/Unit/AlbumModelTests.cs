using FluentAssertions;
using PSI.Models;

namespace Tests.Unit;

public class AlbumModelTests
{
    [Fact]
    public void AddSong_DoesNotDuplicate()
    {
        var album = new Album("A","B",2000);
        var song = new Song { Id = Guid.NewGuid(), Title = "X" };

        album.AddSong(song);
        album.AddSong(song);

        album.Songs.Should().HaveCount(1);
        album.GetAllSongs().Should().ContainSingle(s => s.Id == song.Id);
    }

    [Fact]
    public void IncreasePopularity_Increments()
    {
        var album = new Album("A","B",2000);
        album.Popularity.Should().Be(0);
        album.IncreasePopularity();
        album.Popularity.Should().Be(1);
    }
}
