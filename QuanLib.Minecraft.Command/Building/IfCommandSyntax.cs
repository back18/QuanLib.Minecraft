using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfCommandSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        public DimensionSyntax Dimension()
        {
            SetSyntax("dimension");
            return new DimensionSyntax(this);
        }

        public BlockPositionSyntax<BiomeSyntax<ExecuteCommandSyntax>> Biome()
        {
            SetSyntax("biome");
            return new BlockPositionSyntax<BiomeSyntax<ExecuteCommandSyntax>>(this);
        }

        public BlockPositionSyntax<BlockSyntax<ExecuteCommandSyntax>> Block()
        {
            SetSyntax("block");
            return new BlockPositionSyntax<BlockSyntax<ExecuteCommandSyntax>>(this);
        }

        public SelectorSyntax<ExecuteCommandSyntax> Entity()
        {
            SetSyntax("entity");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        public IfBlocksCommandSyntax Blocks()
        {
            SetSyntax("blocks");
            return new IfBlocksCommandSyntax(this);
        }

        public BlockPositionSyntax<ExecuteCommandSyntax> Loaded()
        {
            SetSyntax("loaded");
            return new BlockPositionSyntax<ExecuteCommandSyntax>(this);
        }

        public IfScoreCompareSyntax ScoreCompare()
        {
            SetSyntax("score");
            return new IfScoreCompareSyntax(this);
        }

        public IfScoreMatchesSyntax ScoreMatches()
        {
            SetSyntax("score");
            return new IfScoreMatchesSyntax(this);
        }

        public IfBlcokDataSyntax BlockData()
        {
            SetSyntax("data block");
            return new IfBlcokDataSyntax(this);
        }

        public IfEntityDataSyntax EntityData()
        {
            SetSyntax("data entity");
            return new IfEntityDataSyntax(this);
        }
    }
}
