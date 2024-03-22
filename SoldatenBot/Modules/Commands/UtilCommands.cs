using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SoldatenBot.Modules.Interactions;
using SoldatenBot.Modules.Internal;

namespace SoldatenBot.Modules.Commands
{
    public class UtilCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("avatar", "Obten el avatar de un usuario")]
        public async Task AvatarAsync([Summary("usuario", "Usuario del que obtener el avatar")] SocketUser user = null)
        {
            var targetUser = user ?? Context.User;
            
            var replyEmbed = new EmbedBuilder();
            replyEmbed.WithTitle($"Avatar de {targetUser.Username}");
            replyEmbed.Color = new Color(34, 102, 153);
            replyEmbed.WithFooter(EmbedUtils.Footer);
            replyEmbed.WithCurrentTimestamp();
            var userAvatar = targetUser.GetAvatarUrl(size: 2048);
            
            if (Context.User is SocketGuildUser)
            {
                var guildUser = targetUser as SocketGuildUser;

                var serverAvatar = guildUser?.GetGuildAvatarUrl(size: 2048);

                if (serverAvatar != null)
                {
                    if (userAvatar != null)
                    {
                        replyEmbed.Description = $"Esta es la imagen de servidor de {guildUser.Mention}.\n" +
                                                 $"[Global Profile Picture]({userAvatar})";
                    }
                    replyEmbed.ImageUrl = serverAvatar;

                    await ReplyAsync(embed: replyEmbed.Build());
                }
            }
            if (userAvatar == null)
                await RespondAsync("Error al obtener el avatar del usuario especificado \n" +
                                   "Asegurate de que el usuario tenga un avatar");

            replyEmbed.WithImageUrl(userAvatar);
            await RespondAsync(embed: replyEmbed.Build());
        }

        [SlashCommand("autoroles", "Obten tus roles automaticos")]
        public async Task SendColorAsync()
            => await RespondAsync("Selecciona tus Roles", components: AutoRoleInteraction.MessageComponent, ephemeral: true);
    }
}