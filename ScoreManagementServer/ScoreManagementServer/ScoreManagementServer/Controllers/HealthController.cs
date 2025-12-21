using Microsoft.AspNetCore.Mvc;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public string Get()  // 👈 直接返回字符串
        {
            return "Server is healthy!";
        }
    }
}