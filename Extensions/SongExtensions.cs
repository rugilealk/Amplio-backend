using PSI.Models;

namespace PSI.Models
{
    public static class SongExtensions
    {
        public static string GetDisplayName(this Song song)
        {
            return $"{song.Artist} - {song.Title}";
        }
    }
}