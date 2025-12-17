using Microsoft.AspNetCore.Mvc;
using ScoreManagementServer.DTOs;
using ScoreManagementServer.Services;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _authService.Login(request);
            if (result == null)
                return Ok(ApiResponse<LoginResponse>.Fail("用户名或密码错误"));

            return Ok(ApiResponse<LoginResponse>.Ok(result, "登录成功"));
        }
    }
}