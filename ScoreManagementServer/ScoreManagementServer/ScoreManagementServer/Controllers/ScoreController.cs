using Microsoft.AspNetCore.Mvc;
using ScoreManagementServer.Services;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly ScoreService _scoreService;

        public ScoreController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        // POST api/score
        [HttpPost]
        public async Task<IActionResult> SaveScore([FromBody] ScoreRequest request)
        {
            await _scoreService.SaveScoreAsync(request.PlayerName, request.Score);
            return Ok(new { message = "Score saved successfully" });
        }

        // GET api/score/top?limit=10
        [HttpGet("top")]
        public async Task<IActionResult> GetTopScores([FromQuery] int limit = 10)
        {
            var scores = await _scoreService.GetTopScoresAsync(limit);
            return Ok(scores);
        }

        // GET api/score/player/{playerName}
        [HttpGet("player/{playerName}")]
        public async Task<IActionResult> GetPlayerBestScore(string playerName)
        {
            var score = await _scoreService.GetPlayerBestScoreAsync(playerName);
            return score != null ? Ok(score) : NotFound();
        }
    }

    // 请求数据模型
    public class ScoreRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}