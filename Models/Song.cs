namespace PSI.Models;

public class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int Votes { get; set; } = 0;
}
