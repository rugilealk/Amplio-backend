namespace PSI.Models
{
    public class Song
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string FilePath { get; set; } = string.Empty;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

        public Song() { }

        public Song(string Title, string Artist, List<Genre> Genres, string FilePath)
        {
            this.Title = Title;
            this.Artist = Artist;
            this.Genres = Genres;
            this.FilePath = FilePath;
        }

        public Stream OpenStream()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new InvalidOperationException("File path for this song is not set.");

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!System.IO.File.Exists(fullPath))
                throw new FileNotFoundException("Song file not found.", fullPath);

            // Open the file for reading and return the stream
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
