using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PSI.Models
{
    public class Song
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public FilePath FilePath { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
        

        public Stream OpenStream()
        {
            if (string.IsNullOrEmpty(FilePath.Value))
            {
                throw new InvalidOperationException("File path for this song is not set.");
            }

            var fullPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", FilePath.Value);
            fullPath = Path.GetFullPath(fullPath);

            if (!System.IO.File.Exists(fullPath))
                throw new FileNotFoundException("Song file not found." + fullPath);

            // Open the file for reading and return the stream
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
