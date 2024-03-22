using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace SoldatenBot.Modules.Commands
{
    public class ModerationCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ban", "Culea a un usuario del servidor")]
        public async Task BanAsync([Summary("usuario", "Usuario a culear")] SocketGuildUser guildUser,
            [Summary("razon", "Razon del ban")] string banReason = " ")
        {
            var contextUser = (SocketGuildUser) Context.User;
            if (!CheckPermissions(contextUser))
            {
                await RespondAsync("No tienes permisos para usar este comando", ephemeral: true);
                return;
            }
            
            if (banReason == string.Empty)
            {
                await guildUser.BanAsync(7);
                await RespondAsync("Espia de Roier Culeado", ephemeral: true);
                return;
            }
            
            await guildUser.BanAsync(7, banReason);
            
            await RespondAsync($"Espia de Roier Culeado por: {banReason}", ephemeral: true);
        }
        
        [SlashCommand("kick", "Kickea a alguien del servidor")]
        public async Task KickAsync([Summary("usuario", "Usuario a kickear")] SocketGuildUser guildUser,
            [Summary("razon", "Razon del kick")] string kickReason = " ")
        {
            var contextUser = (SocketGuildUser) Context.User;
            if (!CheckPermissions(contextUser))
            {
                await RespondAsync("No tienes permisos para usar este comando", ephemeral: true);
                return;
            }
            
            if (kickReason == string.Empty)
            {
                await guildUser.KickAsync();
                await RespondAsync("Espia de Roier Culeado", ephemeral: true);
                return;
            }
            
            await guildUser.KickAsync(kickReason);
            
            await RespondAsync($"Espia de Roier Culeado por: {kickReason}", ephemeral: true);
        }
        
        [SlashCommand("limpieza", "Limpia el chat")]
        public async Task ClearChatAsync([Summary("Canal", "Canal en el que hay que eliminar")] SocketTextChannel channel, 
            [Summary("Cantidad", "Cantidad de mensajes a eliminar")] int amount)
        {
            var contextUser = (SocketGuildUser) Context.User;
            if (!CheckPermissions(contextUser))
            {
                await RespondAsync("No tienes permisos para usar este comando", ephemeral: true);
                return;
            }
            
            var messages = await channel.GetMessagesAsync(amount).FlattenAsync();

            await channel.DeleteMessagesAsync(messages);
            await RespondAsync($"Mensajes eliminados\nCantidad: {amount}", ephemeral: true);
        }
        
        [SlashCommand("mute", "Mutea a un usuario")]
        public async Task MuteAsync([Summary("usuario", "Usuario a mutear")] SocketGuildUser guildUser)
        {
            var contextUser = (SocketGuildUser) Context.User;
            if (!CheckPermissions(contextUser))
            {
                await RespondAsync("No tienes permisos para usar este comando", ephemeral: true);
                return;
            }
            
            var role = guildUser.Guild.Roles.FirstOrDefault(x => x.Name == "Oprimido");
            await guildUser.AddRoleAsync(role);
            await RespondAsync($"Usuario {guildUser.Mention} muteado", ephemeral: true);
        }
        
        private static bool CheckPermissions(SocketGuildUser user)
        {
            return user.Roles.Any(x => x.Id == 1158787534428569760);
        }
    }
}