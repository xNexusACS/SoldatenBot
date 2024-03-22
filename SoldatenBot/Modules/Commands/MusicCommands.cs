using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;

namespace SoldatenBot.Modules.Commands
{
    [Group("musica", "Comandos de musica")]
    public class MusicCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IAudioService _audioService;

        public MusicCommands(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        [SlashCommand("unirme", "Mete al bot a un canal de voz")]
        public async Task JoinVcAsync()
        {
            var embed = new EmbedBuilder().WithColor(102, 196, 166);
            var userVoiceState = (Context.User as IVoiceState)!;

            if (userVoiceState.VoiceChannel is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estas en un canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }

            if (_audioService.HasPlayer(Context.Guild.Id) && Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy en tu canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }

            try
            {
                await _audioService.JoinAsync<QueuedLavalinkPlayer>(Context.Guild.Id, userVoiceState.VoiceChannel.Id, true);

                embed.WithAuthor($"✅ Musica: Comenzado, pedido por {Context.User.Username}")
                    .WithTitle($"Me he unido a {userVoiceState.VoiceChannel.Name}.")
                    .WithThumbnailUrl(Context.User.GetAvatarUrl());

                await RespondAsync(embed: embed.Build());
            }
            catch (Exception e)
            {
                await RespondAsync($"He fallado al unirme al canal de voz: {e.Message}", ephemeral: true);
            }
        }

        [SlashCommand("play", "Reproduce un audio")]
        public async Task PlayAudioAsync([Summary("audio", "Link del audio")] string vibe)
        {
            var embed = new EmbedBuilder().WithColor(102, 196, 166);
            
            var userVoiceState = (Context.User as IVoiceState)!;

            if (userVoiceState.VoiceChannel is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estas en un canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy en tu canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            var audioPlayer = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;
            var track = await _audioService.GetTrackAsync(vibe, SearchMode.YouTube | SearchMode.SoundCloud);
            
            if (track is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No se ha encontrado el audio");

                await RespondAsync(embed: embed.Build());
                return;
            }
        
            var pos = await audioPlayer.PlayAsync(track);
            
            embed.WithAuthor(pos > 0
                ? $"✅ Musica: Audio añadido a la queue, pedido por {Context.User.Username}"
                : $"✅ Reproduciendo el audio, pedido por {Context.User.Username}");

            embed.WithTitle(track.Title)
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .AddField("Autor", track.Author, true)
                .AddField("Duracion", track.Duration, true)
                .AddField("Posicion en Queue", pos, true);
        
            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("queue", "Muestra la queue de musica")]
        public async Task ShowMusicQueueAsync()
        {
            var embed = new EmbedBuilder().WithColor(102, 196, 166);

            if (!_audioService.HasPlayer(Context.Guild.Id))
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No hay musica en reproduccion");

                await RespondAsync(embed: embed.Build());
                return;
            }
        
            var userVoiceState = (Context.User as IVoiceState)!;

            if (userVoiceState.VoiceChannel is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estas en un canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }

            if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy en tu canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            var audioPlayer = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

            if (audioPlayer.CurrentTrack is null)
            {
                embed.WithAuthor("❌ Music: Error")
                    .WithTitle("The queue is empty");

                await RespondAsync(embed: embed.Build());
                return;
            }

            embed.WithTitle("Queue")
                .AddField($"Reproduciendo: {audioPlayer.CurrentTrack!.Title}", $"{audioPlayer.CurrentTrack.Author} - {audioPlayer.CurrentTrack.Duration}");

            int pos = 1;
            foreach (var vibe in audioPlayer.Queue)
                embed.AddField($"[{pos++}]. {vibe.Title}", $"{vibe.Author} - {vibe.Duration}");

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("saltar", "Salta el audio actual")]
        public async Task SkipMusicAsync()
        {
            var embed = new EmbedBuilder().WithColor(102, 196, 166);

            if (!_audioService.HasPlayer(Context.Guild.Id))
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No hay musica en reproduccion");

                await RespondAsync(embed: embed.Build());
                return;
            }

            var userVoiceState = (Context.User as IVoiceState)!;

            if (userVoiceState.VoiceChannel is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estas en un canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy en tu canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
        
            var audioPlayer = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

            if (audioPlayer.State == PlayerState.NotPlaying)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No hay musica en reproduccion");

                await RespondAsync(embed: embed.Build());
                return;
            }

            var skippedVibe = audioPlayer.CurrentTrack!;

            audioPlayer.LoopMode = PlayerLoopMode.None;

            await audioPlayer.SkipAsync();

            embed.WithAuthor($"✅ Audio saltado por {Context.User.Username}")
                .WithTitle(skippedVibe.Title)
                .AddField("Nuevo Audio", audioPlayer.CurrentTrack?.Title ?? "-")
                .WithThumbnailUrl(Context.User.GetAvatarUrl());

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("loop", "Repite el audio actual infinitamente")]
        public async Task LoopAsync()
        {
            var embed = new EmbedBuilder().WithColor(102, 196, 166);

            if (!_audioService.HasPlayer(Context.Guild.Id))
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy reproduciendo musica");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            var userVoiceState = (Context.User as IVoiceState)!;

            if (userVoiceState.VoiceChannel is null)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy en un canal de voz");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            if (Context.Guild.CurrentUser.VoiceChannel.Id != userVoiceState.VoiceChannel.Id)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("Debes estar en el mismo canal de voz que yo");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            var audioPlayer = _audioService.GetPlayer<QueuedLavalinkPlayer>(Context.Guild.Id)!;

            if (audioPlayer.State == PlayerState.NotPlaying)
            {
                embed.WithAuthor("❌ Musica: Error")
                    .WithTitle("No estoy reproduciendo musica");

                await RespondAsync(embed: embed.Build());
                return;
            }
            
            audioPlayer.LoopMode = audioPlayer.LoopMode is PlayerLoopMode.Track
                ? PlayerLoopMode.None
                : PlayerLoopMode.Track;

            if (audioPlayer.LoopMode is PlayerLoopMode.Track)
            {
                embed.WithAuthor($"🔁 Audio en Loop, pedido por: {Context.User.Username}")
                    .WithTitle("Audio en Loop")
                    .WithThumbnailUrl(Context.User.GetAvatarUrl());
            }
            else
            {
                embed.WithAuthor($"🔁 El Audio ya no esta en Loop, pedido por: {Context.User.Username}")
                    .WithTitle("Audio ya no esta en Loop")
                    .WithThumbnailUrl(Context.User.GetAvatarUrl());
            }

            await RespondAsync(embed: embed.Build());
        }
    }
}