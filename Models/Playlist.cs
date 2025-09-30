namespace PSI.Models
{
	public class Playlist
	{
		private List<PlaylistSong> songs;
		public readonly Guid PlaylistId;

		public Playlist()
		{
			songs = new List<PlaylistSong>();
			PlaylistId = Guid.NewGuid();
		}

		public void AddSong(Song newSong)
		{
			if (!songs.Any(playlistSong => playlistSong.Song.Id == newSong.Id))
			{
            	songs.Add(new PlaylistSong(newSong));
			}
		}

		public List<PlaylistSong> GetAllSongs() => songs;

		public PlaylistSong? GetSongById(Guid id)
		{
			return songs.FirstOrDefault(playlistSong => playlistSong.Song.Id == id);
		}

		public bool DeleteSong(Guid id)
		{
			var playlistSong = GetSongById(id);
			if (playlistSong == null) return false;
			return songs.Remove(playlistSong);
		}

		public int ReturnCount()
		{
			return songs.Count;
		}

		public void UpvoteSong(Guid id)
        {
            var playlistSong = GetSongById(id);
            if (playlistSong != null)
            {
                playlistSong.Upvote();
                SortSongs();
            }
        }

		private void SortSongs()
		{
			songs = songs.OrderByDescending(playlistSong => playlistSong.Votes).ToList();
		}

}
}
