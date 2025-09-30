using PSI.Models;

namespace PSI.Services;

public class PlaylistService
{
    private readonly List<Playlist> playlist = new();

    public Playlist Create()
    {
        var playlist = new Playlist();
        this.playlist.Add(playlist);
        return playlist;
    }

    public Playlist? GetById(Guid id) =>
        playlist.FirstOrDefault(playlist => playlist.PlaylistId == id);
}
