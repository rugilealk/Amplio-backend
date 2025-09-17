using PSI.Models;

namespace PSI.Services;

public class PlaylistService
{
    private readonly List<Playlist> Playlists = new();

    public Playlist Create()
    {
        var p = new Playlist();
        Playlists.Add(p);
        return p;
    }

    public Playlist? Get(Guid id) =>
        Playlists.FirstOrDefault(p => p.PlaylistId == id);
}
