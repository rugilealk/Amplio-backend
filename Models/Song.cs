namespace PSI.Models;

public class Song
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int Votes { get; private set; } = 0;
    public Song(string title, string artist)
    {
        Title = title;
        Artist = artist;
    }
    public void Upvote() => Votes++;
}
