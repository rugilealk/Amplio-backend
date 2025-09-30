using PSI.Models;
using Microsoft.EntityFrameworkCore;
using PSI.Data;

namespace PSI.Services
{

public class SongService
{
    private readonly AppDbContext databaseContext;

    public SongService(AppDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    /*public SongService()
    {
        songs = new List<Song>
        {
            new Song("Everlong", "Foo Fighters"),
            new Song("Billie Jean", "Michael Jackson"),
            new Song("Bohemian Rhapsody", "Queen"),
            new Song("Shake It Off", "Taylor Swift"),
            new Song("Smells Like Teen Spirit", "Nirvana")
        };
    }*/

    public async Task<IEnumerable<Song>> GetAllSongsAsync()
    {
        return await databaseContext.Songs.ToListAsync();
    }

    public async Task<Song?> GetSongByIdAsync(Guid songId)
    {
        return await databaseContext.Songs.FindAsync(songId);
    }

    public async Task<Song> CreateSongAsync(string songTitle, string songArtist)
    {
        var newSong = new Song(songTitle, songArtist);
        databaseContext.Songs.Add(newSong);
        await databaseContext.SaveChangesAsync();
        return newSong;
    }
}
}
