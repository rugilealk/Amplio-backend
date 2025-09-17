using PSI.Models;

namespace PSI.Services;

public class SongService
{
    private readonly List<Song> Songs;

    public SongService()
    {
        Songs = new List<Song>
        {
            new Song("Everlong", "Foo Fighters"),
            new Song("Billie Jean", "Michael Jackson"),
            new Song("Bohemian Rhapsody", "Queen"),
            new Song("Shake It Off", "Taylor Swift"),
            new Song("Smells Like Teen Spirit", "Nirvana")
        };
    }

    public IEnumerable<Song> GetAll() => Songs;

    public Song? GetById(Guid id) =>
        Songs.FirstOrDefault(s => s.Id == id);
}
