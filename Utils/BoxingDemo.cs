using System;
using PSI.Models;

namespace PSI.Utils
{
    public static class BoxingDemo
    {
        //boxing and unboxing for Genre using a Song instace
        public static void DemonstrateGenreBoxingUnboxing(Song song)
        {
            if (song.Genres.Count == 0)
            {
                Console.WriteLine($"Song '{song.Title}' has no genres to demonstrate boxing/unboxing.");
                return;
            }

            Genre genre = song.Genres[0];
            object boxed = genre; // boxing
            Genre unboxed = (Genre)boxed; // unboxing

            Console.WriteLine($"Song: {song.Title} | Genre - Original: {genre}, Boxed: {boxed}, Unboxed: {unboxed}");
        }
    }
}