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

        public static void AddData(ulong userId, int xp, int level)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Levels(UserId, Xp, Level) VALUES(@userId, @xp, @level)";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@xp", xp);
            cmd.Parameters.AddWithValue("@level", level);
            cmd.ExecuteNonQuery();
        }
        
        public static bool ExistInDatabase(ulong userId)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            return cmd.ExecuteScalar() != null;
        }
        
        public static void DeleteData(ulong userId)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }
        
        public static int GetLevel(ulong userId)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Level FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        
        public static int GetXp(ulong userId)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Xp FROM Levels WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        
        public static List<SocketUser> GetTopLevels()
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();
            
            using var cmd = conn.CreateCommand();
            
            cmd.CommandText = "SELECT * FROM Levels ORDER BY Level DESC LIMIT 10";
            
            using var reader = cmd.ExecuteReader();
            
            var users = new List<SocketUser>();
            
            while (reader.Read())
            {
                var userId = reader.GetInt64(0);
                var user = Program.Instance.CurrentGuild.GetUser((ulong)userId);
                users.Add(user);
            }

            return users;
        }
        
        public static void AddXp(ulong userId, int value = 10)
        {
            var currentLevel = GetLevel(userId);
            var xpLimit = currentLevel < 10 ? 1000 : 2000;

            if (GetXp(userId) >= xpLimit)
            {
                LevelUp(userId);
                return;
            }
            
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Levels SET Xp = Xp + @value WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.ExecuteNonQuery();
        }
        
        private static void LevelUp(ulong userId)
        {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Levels SET Level = Level + 1 WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
            
            var levelChannel = Program.Instance.CurrentGuild.GetTextChannel(1209898835908759592);
            levelChannel.SendMessageAsync($"¡Felicidades culero <@{userId}>, has subido de nivel de culeo a {GetLevel(userId)}!");
            
            cmd.CommandText = "UPDATE Levels SET Xp = 0 WHERE UserId = @userId";
            cmd.ExecuteNonQuery();
        }
    }
}
