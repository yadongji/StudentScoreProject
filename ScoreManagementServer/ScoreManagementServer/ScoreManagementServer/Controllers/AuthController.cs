using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoreManagementServer.Data;  // 👈 添加引用
using System.Security.Cryptography;
using System.Text;

namespace ScoreManagementServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _db;

        // 👇 通过依赖注入获取数据库上下文
        public AuthController(GameDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 👇 从数据库查找用户
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "用户名或密码错误" });
            }

            // 验证密码
            if (user.PasswordHash != HashPassword(request.Password))
            {
                return Unauthorized(new { success = false, message = "用户名或密码错误" });
            }

            // 生成 Token
            var token = GenerateToken(user.UserId);

            return Ok(new 
            { 
                success = true,
                token = token,
                userId = user.UserId,
                username = user.Username
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 检查用户是否已存在
            var exists = await _db.Users
                .AnyAsync(u => u.Username == request.Username);

            if (exists)
            {
                return Conflict(new { success = false, message = "用户名已存在" });
            }

            // 创建新用户
            var user = new UserData
            {
                UserId = Guid.NewGuid().ToString(),
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new 
            { 
                success = true,
                message = "注册成功",
                userId = user.UserId
            });
        }

        // 👇 查看所有用户（调试用）
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .Select(u => new 
                { 
                    u.UserId,
                    u.Username,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateToken(string userId)
        {
            var data = $"{userId}:{DateTime.UtcNow.Ticks}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
