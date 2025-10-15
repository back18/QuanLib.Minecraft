using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class BlockSyntax<TNext>(ICommandSyntax? previous) : CommandSyntax(previous), ICreatible<BlockSyntax<ExecuteCommandSyntax>> where TNext : ICreatible<TNext>
    {
        public TNext Air()
        {
            SetSyntax("minecraft:air");
            return TNext.Create(this);
        }

        public TNext SetBlock(string blockId)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            SetSyntax(blockId);
            return TNext.Create(this);
        }

        public static BlockSyntax<ExecuteCommandSyntax> Create(ICommandSyntax? previous)
        {
            return new BlockSyntax<ExecuteCommandSyntax>(previous);
        }
    }
}
