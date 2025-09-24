namespace PSI.Models
{
	public class Playlist
	{
    private List<PlaylistSong> Songs;
		public readonly Guid PlaylistId;

		public Playlist()
		{
			Songs = new List<PlaylistSong>();
			PlaylistId = Guid.NewGuid();
		}

		public void AddSong(Song newSong)
		{
			if (!Songs.Any(ps => ps.Song.Id == newSong.Id))
			{
            	Songs.Add(new PlaylistSong(newSong));
			}
		}

		public List<PlaylistSong> GetAllSongs() => Songs;

		public PlaylistSong? GetSongById(Guid id)
		{
			return Songs.FirstOrDefault(s => s.Song.Id == id);
		}

		public bool DeleteSong(Guid id)
		{
			var ps = GetSongById(id);
			if (ps == null) return false;
			return Songs.Remove(ps);
		}

		public int ReturnCount()
		{
			return Songs.Count;
		}

		public void UpvoteSong(Guid id)
        {
            var ps = GetSongById(id);
            if (ps != null)
            {
                ps.Upvote();
                SortSongs();
            }
        }

		private void SortSongs()
		{
			Songs = Songs.OrderByDescending(s => s.Votes).ToList();
		}

}
}
