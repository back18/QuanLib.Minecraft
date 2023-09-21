using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public class ScoreboardPlayersGetCommand : CommandBase<int>
    {
        public ScoreboardPlayersGetCommand()
        {
            Input = TextTemplate.Parse("scoreboard players get %s %s");
            Output = LanguageManager.Instance["commands.scoreboard.players.get.success"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, string target, string objective, out int result)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));
            if (string.IsNullOrEmpty(objective))
                throw new ArgumentException($"“{nameof(objective)}”不能为 null 或空。", nameof(objective));

            return base.TrySendCommand(sender, new object[] { target, objective }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            if (outargs is not null && outargs.Length == 3 && int.TryParse(outargs[1], out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
