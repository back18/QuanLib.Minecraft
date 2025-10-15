using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class BiomeSyntax<TNext>(ICommandSyntax? previous) : CommandSyntax(previous), ICreatible<BiomeSyntax<ExecuteCommandSyntax>> where TNext : ICreatible<TNext>
    {
        public TNext SetBiome(string biomeId)
        {
            ArgumentException.ThrowIfNullOrEmpty(biomeId, nameof(biomeId));

            SetSyntax(biomeId);
            return TNext.Create(this);
        }

        public static BiomeSyntax<ExecuteCommandSyntax> Create(ICommandSyntax? previous)
        {
            return new BiomeSyntax<ExecuteCommandSyntax>(previous);
        }
    }
}
