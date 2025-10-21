namespace PSI.DTOs
{
    public record CreatePlaylistRequestDto(
        string Name,
        Guid? CurrentSongId = null
    );
}
//CurrentSongId - optional argument 