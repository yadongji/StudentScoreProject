using System;
using System.Security.Cryptography;
using System.Text;
using ScoreManagementServer.Database;
using ScoreManagementServer.DTOs;
using ScoreManagementServer.Models;

namespace ScoreManagementServer.Services
{
    public class AuthService
    {
        private readonly DatabaseHelper _db;

        public AuthService(DatabaseHelper db)
        {
            _db = db;
        }

        public LoginResponse Login(LoginRequest request)
        {
            // 验证用户
            var user = _db.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { request.Username });

            if (user == null)
                return null;

            // 验证密码
            var inputPasswordHash = Md5Hash(request.Password);
            if (user.PasswordHash != inputPasswordHash)
                return null;

            // 构建响应
            var response = new LoginResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                RealName = user.RealName,
                Role = user.Role,
                Token = GenerateToken(user.UserId),
                RoleData = GetRoleSpecificData(user)
            };

            return response;
        }

        private RoleSpecificData GetRoleSpecificData(User user)
        {
            var roleData = new RoleSpecificData();

            switch (user.Role)
            {
                case "Student":
                    var student = _db.QueryFirstOrDefault<Student>(
                        "SELECT * FROM Students WHERE UserId = @UserId",
                        new { user.UserId });
                    if (student != null)
                    {
                        roleData.StudentId = student.StudentId;
                        roleData.ClassId = student.ClassId;
                        var classInfo = _db.QueryFirstOrDefault<Class>(
                            "SELECT * FROM Classes WHERE ClassId = @ClassId",
                            new { student.ClassId });
                        roleData.GradeId = classInfo?.GradeId;
                    }
                    break;

                case "Teacher":
                    var managedClasses = _db.Query<int>(
                        "SELECT ClassId FROM Classes WHERE TeacherId = (SELECT TeacherId FROM Teachers WHERE UserId = @UserId)",
                        new { user.UserId });
                    roleData.ManagedClassIds = managedClasses;
                    break;

                case "GradeDirector":
                    var director = _db.QueryFirstOrDefault<GradeDirector>(
                        "SELECT * FROM GradeDirectors WHERE UserId = @UserId",
                        new { user.UserId });
                    roleData.GradeId = director?.GradeId;
                    break;
            }

            return roleData;
        }

        private string GenerateToken(int userId)
        {
            // 简单token生成（生产环境应使用JWT）
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var raw = $"{userId}:{timestamp}:{Guid.NewGuid()}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        }

        private string Md5Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}
