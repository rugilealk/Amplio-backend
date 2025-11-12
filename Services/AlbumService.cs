using Microsoft.EntityFrameworkCore;
using PSI.Data;
using PSI.Models;
namespace PSI.Services
{
    public class AlbumService
    {
        private readonly AppDbContext _databaseContext;

        public AlbumService(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<List<Album>> GetAllAlbumsAsync()
        {
            return await _databaseContext.Albums
                .Include(a => a.Songs)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Album?> GetAlbumByIdAsync(Guid albumId)
        {
            return await _databaseContext.Albums
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == albumId);
        }
        public async Task<Album> CreateAlbumAsync(string name, string artist, int releaseYear)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Album name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(artist))
                throw new ArgumentException("Artist name cannot be empty", nameof(artist));
            
            if (releaseYear > DateTime.Now.Year)
                throw new ArgumentOutOfRangeException(nameof(releaseYear), "Release year is out of valid range.");

            var album = new Album(name, artist, releaseYear);

            _databaseContext.Albums.Add(album);
            await _databaseContext.SaveChangesAsync();

            return album;
        }

        public async Task AddSongToAlbumAsync(Guid albumId, Guid songId)
        {
            var album = await GetAlbumByIdAsync(albumId)
                ?? throw new KeyNotFoundException("Album not found");

            var song = await _databaseContext.Songs.FindAsync(songId)
                ?? throw new KeyNotFoundException("Song not found");

            album.AddSong(song);
            song.AlbumId = albumId;
            song.Album = album;

            await _databaseContext.SaveChangesAsync();
        }

        public async Task IncreaseAlbumPopularityAsync(Guid albumId)
        {
            var album = await GetAlbumByIdAsync(albumId);
            if (album != null)
            {
                album.IncreasePopularity();
                await _databaseContext.SaveChangesAsync();
            }
        }
    }
}
