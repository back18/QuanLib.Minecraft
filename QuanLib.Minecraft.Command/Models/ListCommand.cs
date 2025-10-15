using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ListCommand : CommandBase<PlayerList>, ICreatible<ListCommand>
    {
        public ListCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.list.players"];
            Input = TextTemplate.Parse("list");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, [MaybeNullWhen(false)] out PlayerList result)
        {
            return base.TrySendCommand(sender, Array.Empty<object>(), out result);
        }

        protected override bool TryParseResult(string[] args, [MaybeNullWhen(false)] out PlayerList result)
        {
            if (args is null || args.Length != 3)
                goto fail;

            if (!int.TryParse(args[0], out var onlinePlayers))
                goto fail;
            if (!int.TryParse(args[1], out var maxPlayers))
                goto fail;
            string[] list = args[2].Split(", ");

            result = new(onlinePlayers, maxPlayers, list);
            return true;

            fail:
            result = null;
            return false;
        }

        public static ListCommand Create(LanguageManager languageManager)
        {
            return new ListCommand(languageManager);
        }
    }
}
