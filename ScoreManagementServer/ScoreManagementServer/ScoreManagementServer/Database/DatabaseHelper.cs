using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;

namespace ScoreManagementServer.Database
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }

        public IDbConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        // 查询单个对象
        public T QueryFirstOrDefault<T>(string sql, object param = null)
        {
            using var conn = GetConnection();
            return conn.QueryFirstOrDefault<T>(sql, param);
        }

        // 查询列表
        public List<T> Query<T>(string sql, object param = null)
        {
            using var conn = GetConnection();
            return conn.Query<T>(sql, param).AsList();
        }

        // 执行命令（INSERT/UPDATE/DELETE）
        public int Execute(string sql, object param = null)
        {
            using var conn = GetConnection();
            return conn.Execute(sql, param);
        }

        // 事务执行
        public void ExecuteTransaction(Action<IDbConnection, IDbTransaction> action)
        {
            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                action(conn, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}