using PSI.Models;

namespace PSI.Services;

public class PlaylistService
{
    private readonly List<Song> _songs = [];

    public IEnumerable<Song> GetAll() =>
        _songs.OrderByDescending(s => s.Votes).ThenBy(s => s.Title);

    public void AddSong(Song song)
    {
        _songs.Add(song);
    }

    public bool Upvote(Guid id)
    {
        var s = _songs.FirstOrDefault(x => x.Id == id);
        if (s is null) return false;
        s.Upvote();
        return true;
    }

    public Song? GetById(Guid id)
    {
        return _songs.FirstOrDefault(s => s.Id == id);
    }

    public bool DeleteSong(Guid id)
    {
        var song = GetById(id);
        if (song == null) return false;
        return _songs.Remove(song);
    }

    public int ReturnCount()
    {
        return _songs.Count;
    }
}
