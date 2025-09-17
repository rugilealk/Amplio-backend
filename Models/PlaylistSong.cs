namespace PSI.Models;

public class PlaylistSong
{
    public Song Song { get; }
    public int Votes { get; private set; }

    public PlaylistSong(Song song)
    {
        Song = song;
    }

    public void Upvote() => Votes++;
}
