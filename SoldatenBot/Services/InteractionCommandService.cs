using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SoldatenBot.Modules.Internal;

namespace SoldatenBot.Services
{
    public class InteractionCommandService
    {
        private readonly InteractionService _service;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _provider;

        public InteractionCommandService(InteractionService service, DiscordSocketClient client, IServiceProvider provider)
        {
            _service = service;
            _client = client;
            _provider = provider;
        }
    
        public async Task LoadInteractions()
        {
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            _client.InteractionCreated += HandleInteractionExecution;
            _service.SlashCommandExecuted += HandleSlashCommandExecution;
            _service.ComponentCommandExecuted += HandleComponentCommandExecution;
        }
        
        private async Task HandleInteractionExecution(SocketInteraction interaction)
        {
            try
            {
                SocketInteractionContext context = new(_client, interaction);
                await _service.ExecuteCommandAsync(context, _provider);
            }
            catch (Exception ex)
            {
                if (interaction.Type is InteractionType.ApplicationCommand)
                    Console.WriteLine(ex);

                await interaction.RespondAsync(embed: await EmbedUtils.CreateErrorEmbed(
                    "Interaction Command Service", "Ha ocurrido un error en la ejecucion de el comando"), ephemeral: true);
            }
        }

        private async Task HandleSlashCommandExecution(SlashCommandInfo info, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                if (result.Error is InteractionCommandError.UnknownCommand)
                    return;
            
                await context.Interaction.RespondAsync(embed: await EmbedUtils.CreateErrorEmbed(
                    "Interaction Command Service", "Ha ocurrido un error al ejecutar el comando"), ephemeral: true);
            }
        }
    
        private async Task HandleComponentCommandExecution(ComponentCommandInfo info, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                if (result.Error is InteractionCommandError.UnknownCommand)
                    return;

                await context.Interaction.RespondAsync(embed: await EmbedUtils.CreateErrorEmbed(
                    "Interaction Command Service", "Ha ocurrido un error al ejecutar el componente."), ephemeral: true);
            }
        }
    }
}