﻿using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public abstract class ConditionalCountCommandBase : CommandBase<int>
    {
        protected ConditionalCountCommandBase(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.execute.conditional.pass_count"];
        }

        public override TextTemplate Output { get; }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            if (outargs is not null && outargs.Length == 1 && int.TryParse(outargs[0], out result))
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
