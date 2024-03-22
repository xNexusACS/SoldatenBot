using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using SkiaSharp;
using SoldatenBot.Services;

namespace SoldatenBot.Modules.Commands
{
    [Group("nivel", "Comandos del sistema de niveles")]
    public class LevelingCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("leaderboard", "Muestra los 10 mejores usuarios en el servidor en level")]
        public async Task LeaderboardAsync() => await GenerateLeaderboard();
        
        [SlashCommand("mostrar", "Muestra tu nivel actual")]
        public async Task LevelAsync([Summary("Usuario", "El usuario al que quieres ver el nivel")] SocketUser socketUser = null)
        {
            var user = socketUser ?? Context.User;
            
            if (!DatabaseService.ExistInDatabase(user.Id))
            {
                await RespondAsync("No tienes nivel, te falta calle", ephemeral: true);
                return;
            }
            
            var imageStream = await CreateUserLevelImage(user);
            await RespondWithFileAsync(imageStream, $"{user.Username}_level.png");
        }
        
        [SlashCommand("darxp", "Da XP a un usuario")]
        public async Task GiveXpAsync([Summary("Usuario", "Usuario al que dar XP")] SocketGuildUser guildUser, [Summary("Cantidad", "Cantidad de XP a dar")] int amount)
        {
            var contextUser = (SocketGuildUser) Context.User;
            if (!CheckPermissions(contextUser))
            {
                await RespondAsync("No tienes permisos para usar este comando", ephemeral: true);
                return;
            }
            
            if (guildUser.IsBot)
            {
                await RespondAsync("No puedes dar XP a un bot", ephemeral: true);
                return;
            }
            
            if (amount < 0)
            {
                await RespondAsync("No puedes dar una cantidad negativa de XP");
                return;
            }

            if (!DatabaseService.ExistInDatabase(guildUser.Id))
            {
                DatabaseService.AddData(guildUser.Id, amount, 1);
                await RespondAsync($"Se han dado {amount} XP a {guildUser.Mention}", ephemeral: true);
                return;
            }
            
            DatabaseService.AddXp(guildUser.Id, amount);
            await RespondAsync($"Se han dado {amount} XP a {guildUser.Mention}", ephemeral: true);
        }
        
        private static bool CheckPermissions(SocketGuildUser user)
        {
            return user.Roles.Any(x => x.Id == 1158787534428569760);
        }
        
        private async Task<Stream> CreateUserLevelImage(SocketUser user)
        {
            const int width = 400;
            const int height = 150;

            using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);

            canvas.Clear(SKColors.Black);

            using var paint = new SKPaint();
            paint.Color = SKColors.White;
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.TextAlign = SKTextAlign.Left;
            paint.FakeBoldText = true;
            paint.TextSize = 30;

            using var underlinePaint = new SKPaint();
            underlinePaint.Color = SKColors.White;
            underlinePaint.StrokeWidth = 2;

            using var webClient = new WebClient();
            var avatarData = webClient.DownloadData(user.GetAvatarUrl());

            using var avatarStream = new MemoryStream(avatarData);
            using var avatarBitmap = SKBitmap.Decode(avatarStream);

            var avatarRect = new SKRect(20, 20, 120, 120);
            canvas.DrawBitmap(avatarBitmap, avatarRect);

            var usernamePoint = new SKPoint(130, 60);
            canvas.DrawText(user.Username, usernamePoint, paint);

            var underlineStartPoint = new SKPoint(130, 65);
            var underlineEndPoint = new SKPoint(130 + paint.MeasureText(user.Username), 65);
            canvas.DrawLine(underlineStartPoint, underlineEndPoint, underlinePaint);

            var level = DatabaseService.GetLevel(user.Id);
            var xp = DatabaseService.GetXp(user.Id);

            var levelPoint = new SKPoint(130, 90);
            canvas.DrawText($"Nivel: {level}", levelPoint, paint);

            var xpPoint = new SKPoint(130, 120);
            canvas.DrawText($"XP: {xp} / 1000", xpPoint, paint);

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return new MemoryStream(data.ToArray());
        }

        private async Task GenerateLeaderboard()
        {
            var users = DatabaseService.GetTopLevels();
            
            const int width = 500;
            var height = users.Count * 60 + 60;
            
            using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);
            
            canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint();
            paint.Color = SKColors.Black;
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.TextAlign = SKTextAlign.Left;
            paint.FakeBoldText = true;

            using var font = new SKFont();
            font.Size = 20;
            font.Edging = SKFontEdging.Antialias;

            using var paintGrey = new SKPaint();
            paintGrey.Color = SKColors.Gray;
            paintGrey.Style = SKPaintStyle.Fill;

            using var paintFirst = new SKPaint(font);
            paintFirst.Color = SKColors.Yellow;
            using var paintSecond = new SKPaint(font);
            paintSecond.Color = SKColors.Orange;
            using var paintThird = new SKPaint(font);
            paintThird.Color = SKColors.Brown;

            for (var i = 0; i < users.Count; i++)
            {
                if (users[i] == null) continue;
                
                if (i > 0 && users[i].Id == users[i - 1].Id) continue;

                var text = $"{users[i].Username} - Nivel {DatabaseService.GetLevel(users[i].Id)} - XP {DatabaseService.GetXp(users[i].Id)}";

                var paintToUse = i switch
                {
                    0 => paintFirst,
                    1 => paintSecond,
                    2 => paintThird,
                    _ => paint
                };

                var rect = new SKRect(40, (i + 2) * 30 - 20 + i * 10, width, (i + 2) * 30 + 10 + i * 10);
                canvas.DrawRect(rect, paintGrey);
                    
                paintToUse.TextAlign = SKTextAlign.Center;
                var textMiddlePoint = new SKPoint(rect.MidX, rect.MidY);
                canvas.DrawText(text, textMiddlePoint.X, textMiddlePoint.Y, paintToUse);

                using var webClient = new WebClient();
                var avatarData = webClient.DownloadData(users[i].GetAvatarUrl());

                using var avatarStream = new MemoryStream(avatarData);
                using var avatarBitmap = SKBitmap.Decode(avatarStream);

                var avatarRect = new SKRect(40, (i + 2) * 30 - 20 + i * 10, 70, (i + 2) * 30 + 10 + i * 10);
                canvas.DrawBitmap(avatarBitmap, avatarRect);
            }
            
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream(data.ToArray());
            
            await RespondWithFileAsync(stream, "leaderboard.png");
        }
    }
}