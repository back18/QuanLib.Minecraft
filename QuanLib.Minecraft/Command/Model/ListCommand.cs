﻿using QuanLib.Core.ExceptionHelper;
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
    public class ListCommand : CommandBase<PlayerList>
    {
        public ListCommand()
        {
            Input = TextTemplate.Parse("list");
            Output = LanguageManager.Instance["commands.list.players"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, [MaybeNullWhen(false)] out PlayerList result)
        {
            return base.TrySendCommand(sender, Array.Empty<object>(), out result);
        }

        public override bool TryParseResult(string[] args, [MaybeNullWhen(false)] out PlayerList result)
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
    }
}