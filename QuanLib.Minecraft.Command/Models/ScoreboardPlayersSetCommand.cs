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
    public class ScoreboardPlayersSetCommand : MultipleCommandBase, ICreatible<ScoreboardPlayersSetCommand>
    {
        public ScoreboardPlayersSetCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            MultipleOutput = languageManager["commands.scoreboard.players.set.success.multiple"];
            Output = languageManager["commands.scoreboard.players.set.success.single"];
            Input = TextTemplate.Parse("scoreboard players set %s %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate MultipleOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, string objective, int score, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));
            ArgumentException.ThrowIfNullOrEmpty(objective, nameof(objective));

            return base.TrySendCommand(sender, [target, objective, score], out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 3, 1, out result);
        }

        public static ScoreboardPlayersSetCommand Create(LanguageManager languageManager)
        {
            return new ScoreboardPlayersSetCommand(languageManager);
        }
    }
}
