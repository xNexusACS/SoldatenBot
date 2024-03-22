using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;

namespace SoldatenBot.Services
{
    public static class DatabaseService
    {
        private static readonly string ConnectionString = $"Data Source={Program.Instance.DatabaseFile}";

        public static async Task OpenDatabase()
        {
            if (File.Exists(Program.Instance.DatabaseFile)) return;
            
            File.Create(Program.Instance.DatabaseFile);
                
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Levels(UserId INTEGER, Xp INTEGER, Level INTEGER)";
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task AddData(ulong userId, int xp, int level)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Levels(UserId, Xp, Level) VALUES(@userId, @xp, @level)";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@xp", xp);
            cmd.Parameters.AddWithValue("@level", level);
            await cmd.ExecuteNonQueryAsync();
        }
        
        public static async Task<bool> ExistInDatabase(ulong userId)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        
        public static async Task DeleteData(ulong userId)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            await cmd.ExecuteNonQueryAsync();
        }
        
        public static async Task<int> GetLevel(ulong userId)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Level FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        
        public static async Task<int> GetXp(ulong userId)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Xp FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        
        public static async Task<List<SocketUser>> GetTopLevels()
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();
            
            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = "SELECT * FROM Levels ORDER BY Level DESC LIMIT 10";
            
            await using var reader = await cmd.ExecuteReaderAsync();
            
            var users = new List<SocketUser>();
            
            while (reader.Read())
            {
                var userId = reader.GetInt64(0);
                var user = Program.Instance.CurrentGuild.GetUser((ulong)userId);
                users.Add(user);
            }

            return users;
        }
        
        public static async Task AddXp(ulong userId, int value = 10)
        {
            var currentLevel = await GetLevel(userId);
            var xpLimit = currentLevel < 10 ? 1000 : 2000;

            if (await GetXp(userId) >= xpLimit)
            {
                await LevelUp(userId);
                return;
            }
            
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Levels SET Xp = Xp + @value WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@value", value);
            await cmd.ExecuteNonQueryAsync();
        }
        
        private static async Task LevelUp(ulong userId)
        {
            await using SqliteConnection conn = new(ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Levels SET Level = Level + 1 WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            await cmd.ExecuteNonQueryAsync();
            
            var levelChannel = Program.Instance.CurrentGuild.GetTextChannel(1209898835908759592);
            await levelChannel.SendMessageAsync($"¡Felicidades culero <@{userId}>, has subido de nivel de culeo a {await GetLevel(userId)}!");
            
            cmd.CommandText = "UPDATE Levels SET Xp = 0 WHERE UserId = @userId";
            await cmd.ExecuteNonQueryAsync();
        }
    }
}