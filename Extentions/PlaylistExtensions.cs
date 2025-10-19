using PSI.Models;

namespace PSI.Extensions
{
    public static class PlaylistExtensions
    {
        // Extension metodas Playlist klasei kuris patikrina ar yra daina su duotu Id ir ja upvotina
        public static bool UpvoteSongById(this Playlist playlist, Guid songId)
        {
            var playlistSong = playlist.GetSongById(songId);
            if (playlistSong == null)
                return false;

            playlistSong.Upvote();
            return true;
        }
    }
}
