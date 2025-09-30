namespace PSI.Models
{
    public class Song
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

        public Song() { }

        public Song(string songTitle, string songArtist)
        {
            Title = songTitle;
            Artist = songArtist;
        }
    }
}
