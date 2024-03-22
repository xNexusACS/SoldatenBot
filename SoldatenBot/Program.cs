using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoldatenBot.Modules.Interactions;
using SoldatenBot.Modules.Internal;
using SoldatenBot.Modules.Server;
using SoldatenBot.Services;

namespace SoldatenBot
{
    public class Program
    {
         private DiscordSocketClient _client;

         public static Task Main() => new Program().MainAsync();

         public SocketGuild CurrentGuild => _client.Guilds.FirstOrDefault(g => g.Id == 967054154822455336)!;
    
         public static Program Instance;

         public string DatabaseFile => Path.Combine(Environment.CurrentDirectory, "BotDatabase.db");

         private Program()
         {
              Instance = this;
         }
    
         private async Task MainAsync()
         {
              var config = new ConfigurationBuilder()
                  .SetBasePath(AppContext.BaseDirectory)
                  .AddYamlFile("config.yml")
                  .Build();
              
              using IHost host = Host.CreateDefaultBuilder()
                  .ConfigureServices((_, services) =>
                      services
                          .AddSingleton(config)
                          .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                          {
                              GatewayIntents = GatewayIntents.All,
                              AlwaysDownloadUsers = true,
                              LogLevel = LogSeverity.Debug,
                              MessageCacheSize = 10000,
                          }))
                          .AddMemoryCache()
                          .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
                          .AddSingleton<IAudioService, LavalinkNode>()
                          .AddSingleton(new LavalinkNodeOptions
                          {
                                RestUri = config["resturi"],
                                WebSocketUri = config["websocketuri"],
                                Password = config["password"],
                          })
                          .AddTransient<ConsoleLogger>()
                          .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                          .AddSingleton<InteractionCommandService>()
                          .AddSingleton<Random>()
                          .AddSingleton<InteractiveService>()
                          .AddSingleton(new CommandService(new CommandServiceConfig
                          { 
                              LogLevel = LogSeverity.Debug,
                              DefaultRunMode = Discord.Commands.RunMode.Async
                          })))
                  .Build();
              
              await RunAsync(host);
         }

         private async Task RunAsync(IHost host)
         {
             using IServiceScope serviceScope = host.Services.CreateScope();
             IServiceProvider provider = serviceScope.ServiceProvider;
         
             var commands = provider.GetRequiredService<InteractionService>();
             _client = provider.GetRequiredService<DiscordSocketClient>();
        
             var logger = provider.GetRequiredService<ConsoleLogger>();
         
             var config = provider.GetRequiredService<IConfigurationRoot>();
         
             await provider.GetRequiredService<InteractionCommandService>().LoadInteractions();
        
             _client.Log += client => logger.Log(client);
             commands.Log += cmd => logger.Log(cmd);

             _client.UserJoined += ServerLogsModule.OnUserJoin;
             _client.UserLeft += ServerLogsModule.OnUserLeft;
             _client.UserBanned += ServerLogsModule.OnUserBanned;
             _client.UserUnbanned += ServerLogsModule.OnUserUnbanned;
             _client.MessageReceived += LevelingHandler.OnMessageSent;
             _client.UserLeft += LevelingHandler.OnUserLeft;
             _client.UserJoined += LevelingHandler.OnUserJoin;
             _client.SelectMenuExecuted += AutoRoleInteraction.HandleSelectMenu;
        
             _client.Ready += async () =>
             {
                 await provider.GetRequiredService<IAudioService>().InitializeAsync();
                 
                 await _client.SetStatusAsync(UserStatus.Online);
                 
                DatabaseService.OpenDatabase();
             
                 Console.WriteLine($"Conectado como => {_client.CurrentUser.Username}");
                 await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["guild"]));
             };
         
             await _client.LoginAsync(TokenType.Bot, config["token"]);
             await _client.StartAsync();
             
             await Task.Delay(-1);
         }
    }
}
