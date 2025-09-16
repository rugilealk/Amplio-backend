using System;

public class Playlist
{
	private List<Song> Songs;
	public Playlist()
	{
		Songs = new List<Song>();
	}
	public void AddSong(Song newSong)
	{
		Songs.Add(newSong);
	}
	public List<Song> GetAllSongs()
	{
		return Songs.OrderByDescending(s => s.Votes).ToList();
	}
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
}
