using System;
using System.Collections.Generic;
using System.Linq;

namespace PSI.Models
{
	public class Playlist
	{
		private List<Song> Songs;
		public readonly Guid PlaylistId;

		public Playlist()
		{
			Songs = new List<Song>();
			PlaylistId = Guid.NewGuid();
		}

		public void AddSong(Song newSong)
		{
			Songs.Add(newSong);
		}

		public List<Song> GetAllSongs() {  return Songs; }

		public Song? GetSongById(Guid id)
		{
			return Songs.FirstOrDefault(s => s.Id == id);
		}

		public bool DeleteSong(Guid id)
		{
			var song = GetSongById(id);
			if (song == null) return false;
			return Songs.Remove(song);
		}

		public int ReturnCount()
		{
			return Songs.Count;
		}

		public void UpvoteSong(Guid id)
        {
            var song = GetSongById(id);
            if (song != null)
            {
                song.Upvote();
                SortSongs();
            }
        }

		private void SortSongs()
		{
			Songs = Songs.OrderByDescending(s => s.Votes).ToList();
		}

}
}
