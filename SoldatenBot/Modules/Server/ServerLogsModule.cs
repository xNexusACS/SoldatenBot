using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SoldatenBot.Modules.Internal;

namespace SoldatenBot.Modules.Server
{
    public static class ServerLogsModule
    {
        public static async Task OnUserJoin(SocketGuildUser user)
        {
            try
            {
                var users = Program.Instance.CurrentGuild.Users;

                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor("Se ha unido un culero", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("**Culero:** " + user.Mention + "\n**ID:** " + user.Id + "\n**Fecha de Creacion:** " +
                                     user.CreatedAt.ToString("dd/MM/yyyy") + "\n**Fecha de union:** " +
                                     DateTime.Now.ToString("dd/MM/yyyy") + "\n**Miembros Totales:** " + users.Count)
                    .WithFooter(EmbedUtils.Footer)
                    .WithCurrentTimestamp()
                    .Build();

                var logChannel = Program.Instance.CurrentGuild.GetTextChannel(1208902093193351268);
                await logChannel.SendMessageAsync(embed: embed);
            
                var welcomeChannel = Program.Instance.CurrentGuild.GetTextChannel(1208900426628472892);
                
                var embed2 = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor("¡Nuevo Culero!", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("¡Un nuevo culero se ha unido al servidor!\n" + user.Mention)
                    .WithFooter(EmbedUtils.Footer)
                    .WithCurrentTimestamp()
                    .WithImageUrl("https://i.imgur.com/yOanHRY.png")
                    .Build();
                
                await welcomeChannel.SendMessageAsync(embed: embed2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task OnUserLeft(SocketGuild guild, SocketUser user)
        {
            try
            {
                var users = Program.Instance.CurrentGuild.Users;

                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor("Se ha ido un espia de roier", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("**Espia de Roier:** " + user.Mention + "\n**ID:** " + user.Id + "\n**Fecha de Creacion:** " +
                                     user.CreatedAt.ToString("dd/MM/yyyy") + "\n**Fecha de salida:** " +
                                     DateTime.Now.ToString("dd/MM/yyyy") + "\n**Miembros Totales:** " + users.Count)
                    .WithFooter(EmbedUtils.Footer)
                    .WithCurrentTimestamp()
                    .Build();
                
                var logChannel = Program.Instance.CurrentGuild.GetTextChannel(1208902093193351268);
                await logChannel.SendMessageAsync(embed: embed);
                
                var leftChannel = Program.Instance.CurrentGuild.GetTextChannel(1208900426628472892);
                
                var embed2 = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor("¡Un espia de Roier se ha ido!", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription($"¡El culero ({user.Mention}) era un espía de Roier y del Estado!")
                    .WithFooter(EmbedUtils.Footer)
                    .WithCurrentTimestamp()
                    .Build();
                
                await leftChannel.SendMessageAsync(embed: embed2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task OnUserBanned(SocketUser user, SocketGuild guild)
        {
            try
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor("Espia de Roier Culeado")
                    .WithDescription("**Espia de Roier:** " + user.Mention + "\n**ID:** " + user.Id +
                                     "\n**Fecha de Culeo:** " +
                                     DateTime.Now.ToString("dd/MM/yyyy"))
                    .WithFooter(EmbedUtils.Footer)
                    .WithCurrentTimestamp()
                    .Build();
                
                var logChannel = Program.Instance.CurrentGuild.GetTextChannel(1208902093193351268);
                await logChannel.SendMessageAsync(embed: embed);
                
                var dmChannel = await user.CreateDMChannelAsync();
                await dmChannel.SendMessageAsync("Has sido culeado del servidor, no vuelvas, espia de Roier.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task OnUserUnbanned(SocketUser user, SocketGuild guild)
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor("Espia de Roier Des-Culeado")
                .WithDescription("**Espia de Roier:** " + user.Mention + "\n**ID:** " + user.Id +
                                 "\n**Fecha de Des-Culeo:** " +
                                 DateTime.Now.ToString("dd/MM/yyyy"))
                .WithFooter(EmbedUtils.Footer)
                .WithCurrentTimestamp()
                .Build();
        }
    }
}