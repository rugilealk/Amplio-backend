using System.Collections;
namespace PSI.Models
{
    public class Playlist : IEnumerable<PlaylistSong>
    {
        public Guid PlaylistId { get; set; } = Guid.NewGuid();
        public Guid? CurrentSongId { get; set; }

        public Song? CurrentSong { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public GenericSongList<PlaylistSong> Songs { get; set; } = new GenericSongList<PlaylistSong>();

        public Playlist(string name)
        {
            this.Name = name;
        }
        public void AddSong(Song songToAdd)
        {
            if (Songs.FindById(songToAdd.Id)==null)
            {
                var newPlaylistSong = new PlaylistSong(songToAdd, this);
                Songs.Add(newPlaylistSong);
            }
        }

        public List<PlaylistSong> GetAllSongs() =>
            Songs.GetOrderedByVotes();

        public PlaylistSong? GetSongById(Guid songId) =>
            Songs.FindById(songId);

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

        public IEnumerator<PlaylistSong> GetEnumerator() => Songs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

