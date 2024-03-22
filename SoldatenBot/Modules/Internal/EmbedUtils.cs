using System.Threading.Tasks;
using Discord;

namespace SoldatenBot.Modules.Internal
{
    public static class EmbedUtils
    {
        public static async Task<Embed> CreateEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithFooter(Footer)
                .WithCurrentTimestamp().Build());
            return embed;
        }
        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"Exception in {source}")
                .WithDescription($"**Error Details**: \n{error}")
                .WithColor(Color.DarkRed)
                .WithFooter(Footer)
                .WithCurrentTimestamp().Build());
            return embed;
        }
        public static string Footer => "Soldaten";
    }
}