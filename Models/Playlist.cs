using System.Collections;
namespace PSI.Models
{
    public class Playlist : IEnumerable<PlaylistSong>
    {
        public Guid PlaylistId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ICollection<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();

        public Playlist(string name)
        {
            this.Name = name;
        }
        public void AddSong(Song songToAdd)
        {
            bool songAlreadyExists = Songs.Any(playlistSong => playlistSong.SongId == songToAdd.Id);
            if (!songAlreadyExists)
            {
                var newPlaylistSong = new PlaylistSong(songToAdd, this);
                Songs.Add(newPlaylistSong);
            }
        }

        public List<PlaylistSong> GetAllSongs() =>
            Songs.OrderByDescending(playlistSong => playlistSong.Votes).ToList();

        public PlaylistSong? GetSongById(Guid songId) =>
            Songs.FirstOrDefault(playlistSong => playlistSong.SongId == songId);

        public bool DeleteSong(Guid songId)
        {
            var playlistSongToRemove = GetSongById(songId);
            if (playlistSongToRemove == null) return false;
            Songs.Remove(playlistSongToRemove);
            return true;
        }

        public void UpvoteSong(Guid songId)
        {
            var playlistSongToUpvote = GetSongById(songId);
            if (playlistSongToUpvote != null) playlistSongToUpvote.Upvote();
        }

        //iterating the right way čia??
        public IEnumerator<PlaylistSong> GetEnumerator() => Songs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

