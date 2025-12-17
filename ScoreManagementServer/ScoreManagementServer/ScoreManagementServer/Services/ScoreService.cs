using Dapper;
using Microsoft.Data.Sqlite;

namespace ScoreManagementServer.Services
{
    public class ScoreService
    {
        private readonly string _connectionString;

        public ScoreService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Scores (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            
            conn.Execute(createTableSql);
        }

        // ⚠️ 保存分数方法
        public async Task<int> SaveScoreAsync(string playerName, int score)
        {
            using var conn = new SqliteConnection(_connectionString);
            var sql = "INSERT INTO Scores (PlayerName, Score) VALUES (@PlayerName, @Score)";
            return await conn.ExecuteAsync(sql, new { PlayerName = playerName, Score = score });
        }

        // ⚠️ 获取排行榜方法
        public async Task<IEnumerable<dynamic>> GetTopScoresAsync(int limit = 10)
        {
            using var conn = new SqliteConnection(_connectionString);
            var sql = "SELECT PlayerName, Score, CreatedAt FROM Scores ORDER BY Score DESC LIMIT @Limit";
            return await conn.QueryAsync(sql, new { Limit = limit });
        }

        // ⚠️ 获取玩家最高分方法
        public async Task<dynamic?> GetPlayerBestScoreAsync(string playerName)
        {
            using var conn = new SqliteConnection(_connectionString);
            var sql = "SELECT PlayerName, Score, CreatedAt FROM Scores WHERE PlayerName = @PlayerName ORDER BY Score DESC LIMIT 1";
            return await conn.QueryFirstOrDefaultAsync(sql, new { PlayerName = playerName });
        }
    }
}
