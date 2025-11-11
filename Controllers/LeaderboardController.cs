using Microsoft.AspNetCore.Mvc;
using PSI.Services;

namespace PSI.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardService _leaderboardService;    
        public LeaderboardController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("playlists")]
        public async Task<IActionResult> GetPlaylistLeaderboard([FromQuery] int topN = 10)
        {
            var leaderboard = await _leaderboardService.GetPlaylistLeaderboardAsync(topN);
            return Ok(leaderboard);
        }

        [HttpGet("albums")]
        public async Task<IActionResult> GetAlbumLeaderboard([FromQuery] int topN = 10)
        {
            var leaderboard = await _leaderboardService.GetAlbumLeaderboardAsync(topN);
            return Ok(leaderboard);
        }
    }
}
