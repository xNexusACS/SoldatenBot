using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using SoldatenBot.Services;

namespace SoldatenBot.Modules.Server
{
    public static class LevelingHandler
    {
        public static async Task OnMessageSent(SocketMessage message)
        {
            try
            {
                if (message.Author.IsBot) return;

                if (message.Author is not SocketGuildUser user) return;
                
                if (await DatabaseService.ExistInDatabase(user.Id))
                {
                    await DatabaseService.AddXp(user.Id);
                    return;
                }
                await DatabaseService.AddData(user.Id, 0, 1);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task OnUserLeft(SocketGuild guild, SocketUser user)
        {
            try
            {
                await DatabaseService.DeleteData(user.Id);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static async Task OnUserJoin(SocketGuildUser user)
        {
            try
            {
                if (await DatabaseService.ExistInDatabase(user.Id)) return;
                
                await DatabaseService.AddData(user.Id, 0, 1);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}