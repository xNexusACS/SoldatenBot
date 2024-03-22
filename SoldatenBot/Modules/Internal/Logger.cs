using System;
using System.Threading.Tasks;
using Discord;
using static System.Guid;

namespace SoldatenBot.Modules.Internal
{
    public abstract class Logger : ILogger
    {
        public string _guid = NewGuid().ToString()[^4..];

        public abstract Task Log(LogMessage message);
    }

    public interface ILogger
    {
        public Task Log(LogMessage message);
    }

    public class ConsoleLogger : Logger
    {
        public override async Task Log(LogMessage message)
        {
            await Task.Run(() => LogToConsoleAsync(this, message));
        }

        private async Task LogToConsoleAsync<T>(T logger, LogMessage message) where T : ILogger
        {
            Console.WriteLine($"guid:{_guid} : " + message);
        }
    }
}