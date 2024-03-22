using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SoldatenBot.Modules.Interactions
{
    public static class AutoRoleInteraction
    {
        private static SelectMenuBuilder SelectMenuOptions { get; } = new SelectMenuBuilder()
            .WithType(ComponentType.SelectMenu)
            .WithMaxValues(1)
            .WithMinValues(0)
            .WithPlaceholder("Colores")
            .WithChannelTypes(ChannelType.Text)
            .WithDisabled(false)
            .WithCustomId("autoRole_Colores")
            .AddOption("Rojo", "1220159273527414834", "Obtienes el Color Rojo")
            .AddOption("Rojo Claro", "1220159256398008350", "Obtienes el Color Rojo Claro")
            .AddOption("Rojo Oscuro", "1220159265017434182", "Obtienes el Color Rojo Oscuro")
            .AddOption("Azul", "1220159903440699532", "Obtienes el Color Azul")
            .AddOption("Azul Claro", "1220159777171181648", "Obtienes el Color Azul Claro")
            .AddOption("Azul Oscuro", "1220159714785103912", "Obtienes el Color Azul Oscuro")
            .AddOption("Verde", "1220160314625228901", "Obtienes el Color Verde")
            .AddOption("Verde Claro", "1220160363333685288", "Obtienes el Color Verde Claro")
            .AddOption("Verde Oscuro", "1220160409177161738", "Obtienes el Color Verde Oscuro")
            .AddOption("Amarillo", "1220160055920296097", "Obtienes el Color Amarillo")
            .AddOption("Amarillo Claro", "1220160132281929768", "Obtienes el Color Amarillo Claro")
            .AddOption("Amarillo Oscuro", "1220160229510217799", "Obtienes el Color Amarillo Oscuro")
            .AddOption("Fucsia", "1220160515544715306", "Obtienes el Color Fucsia")
            .AddOption("Fucsia Claro", "1220160569412161590", "Obtienes el Color Fucsia Claro")
            .AddOption("Fucsia Oscuro", "1220160605269397544", "Obtienes el Color Fucsia Oscuro")
            .AddOption("Celeste", "1220160766372614144", "Obtienes el Color Celeste")
            .AddOption("Celeste Claro", "1220160839902822521", "Obtienes el Color Celeste Claro")
            .AddOption("Celeste Oscuro", "1220160895385210891", "Obtienes el Color Celeste Oscuro")
            .AddOption("Morado", "1220161005678760036", "Obtienes el Color Morado")
            .AddOption("Morado Claro", "1220161053023801355", "Obtienes el Color Morado Claro")
            .AddOption("Morado Oscuro", "1220161103125020692", "Obtienes el Color Morado Oscuro")
            .AddOption("Naranja", "1220161189611307049", "Obtienes el Color Naranja")
            .AddOption("Naranja Claro", "1220161244212887623", "Obtienes el Color Naranja Claro")
            .AddOption("Naranja Oscuro", "1220161318716182549", "Obtienes el Color Naranja Oscuro");
        
        private static SelectMenuBuilder SelectMenuOptions2 { get; } = new SelectMenuBuilder()
            .WithType(ComponentType.SelectMenu)
            .WithMaxValues(1)
            .WithMinValues(0)
            .WithPlaceholder("Colores 2")
            .WithChannelTypes(ChannelType.Text)
            .WithDisabled(false)
            .WithCustomId("autoRole_Colores2")
            .AddOption("Rosa", "1220162173511270411", "Obtienes el Color Rosa")
            .AddOption("Rosa Claro", "1220162249327640690", "Obtienes el Color Rosa Claro")
            .AddOption("Rosa Oscuro", "1220162320765026335", "Obtienes el Color Rosa Oscuro")
            .AddOption("Marron", "1220161978950221955", "Obtienes el Color Marron")
            .AddOption("Negro", "1220161626691604610", "Obtienes el Color Negro")
            .AddOption("Gris", "1220161867817812100", "Obtienes el Color Gris")
            .AddOption("Blanco", "1220161778756092014", "Obtienes el Color Blanco");

        private static ActionRowBuilder ActionRow { get; } = new ActionRowBuilder()
            .WithSelectMenu(SelectMenuOptions);
        
        private static ActionRowBuilder ActionRow2 { get; } = new ActionRowBuilder()
            .WithSelectMenu(SelectMenuOptions2);

        internal static MessageComponent MessageComponent { get; } = new ComponentBuilder()
            .WithRows(new [] {ActionRow, ActionRow2})
            .Build();

        internal static async Task HandleSelectMenu(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "autoRole_Colores":
                    await HandleSelectionsFirst(component);
                    break;
                
                case "autoRole_Colores2":
                    await HandleSelectionsSecond(component);
                    break;
            }
        }

        private static async Task HandleSelectionsFirst(SocketMessageComponent component)
        {
            Console.WriteLine("Handling Selections");

            if (component.User is not SocketGuildUser user) return;

            if (component.Data.Values == null || !component.Data.Values.Any())
            {
                await component.RespondAsync("No seleccionaste ninguna opción", ephemeral: true);
                return;
            }

            foreach (var value in component.Data.Values)
            {
                var role = Program.Instance.CurrentGuild.Roles.FirstOrDefault(x => x.Id == ulong.Parse(value));
                if (role is null) continue;

                if (user.Roles.Any(x => x.Name == role.Name))
                    await user.RemoveRoleAsync(role);
                else
                    await user.AddRoleAsync(role);

                await component.RespondAsync($"Role {role.Name} Actualizado", ephemeral: true);
            }
        }
        
        private static async Task HandleSelectionsSecond(SocketMessageComponent component)
        {
            Console.WriteLine("Handling Selections");

            if (component.User is not SocketGuildUser user) return;

            if (component.Data.Values == null || !component.Data.Values.Any())
            {
                await component.RespondAsync("No seleccionaste ninguna opción", ephemeral: true);
                return;
            }

            foreach (var value in component.Data.Values)
            {
                var role = Program.Instance.CurrentGuild.Roles.FirstOrDefault(x => x.Id == ulong.Parse(value));
                if (role is null) continue;

                if (user.Roles.Any(x => x.Name == role.Name))
                    await user.RemoveRoleAsync(role);
                else
                    await user.AddRoleAsync(role);

                await component.RespondAsync($"Role {role.Name} Actualizado", ephemeral: true);
            }
        }
    }
}
