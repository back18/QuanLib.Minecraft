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
    public class FillCommand : CommandBase<int>, ICreatible<FillCommand>
    {
        public FillCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.fill.success"];
            Input = TextTemplate.Parse("fill %s %s %s %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x1, int y1, int z1, int x2, int y2, int z2, string blockId, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            return base.TrySendCommand(sender, [x1, y1, z1, x2, y2, z2, blockId], out result);
        }

        public static FillCommand Create(LanguageManager languageManager)
        {
            return new FillCommand(languageManager);
        }

        protected override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            if (outargs is not null && outargs.Length == 1)
            {
                return int.TryParse(outargs[0], out result);
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
