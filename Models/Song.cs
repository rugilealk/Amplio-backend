namespace PSI.Models;

public class Song
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public Song(string title, string artist)
    {
        Title = title;
        Artist = artist;
    }
}
