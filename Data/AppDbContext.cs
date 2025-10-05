using Microsoft.EntityFrameworkCore;
using PSI.Models;

namespace PSI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key for join table
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(playlistSong => new { playlistSong.PlaylistId, playlistSong.SongId });

            // Configure relationship: PlaylistSong → Playlist
            modelBuilder.Entity<PlaylistSong>()
                .HasOne(playlistSong => playlistSong.Playlist)
                .WithMany(playlist => playlist.Songs)
                .HasForeignKey(playlistSong => playlistSong.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship: PlaylistSong → Song
            modelBuilder.Entity<PlaylistSong>()
                .HasOne(playlistSong => playlistSong.Song)
                .WithMany(song => song.PlaylistSongs)
                .HasForeignKey(playlistSong => playlistSong.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Configure string property lengths and requirements
            modelBuilder.Entity<Song>()
                .Property(song => song.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Song>()
                .Property(song => song.Artist)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Song>()
               .Property(s => s.FilePath)
               .IsRequired()
               .HasMaxLength(255);
        }
    }
}
