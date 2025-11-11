using System.Transactions;

namespace PSI.Models
{
    public class Album : SongCollection
    {
        public string Artist { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
        public Album(string name, string artist, int releaseYear)
        {
            Name = name;
            Artist = artist;
            ReleaseYear = releaseYear;

        }
        public override void IncreasePopularity()
        {
            popularity ++;
        }
        public override void GetAllSongs() => Songs.ToList();
    }
}
