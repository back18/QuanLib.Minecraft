using QuanLib.Game;
using QuanLib.Minecraft.Command.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building.Extensions
{
    public static class SyntaxExtension
    {
        #region As
        public static ExecuteCommandSyntax As(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.As().SetTarget(selector);
        }

        public static ExecuteCommandSyntax As(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.As().SetTarget(guid);
        }

        public static ExecuteCommandSyntax As(this ExecuteCommandSyntax source, Target target)
        {
            return source.As().SetTarget(target);
        }

        public static ExecuteCommandSyntax As(this ExecuteCommandSyntax source, string target)
        {
            return source.As().SetTarget(target);
        }
        #endregion

        #region At
        public static ExecuteCommandSyntax At(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.At().SetTarget(selector);
        }

        public static ExecuteCommandSyntax At(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.At().SetTarget(guid);
        }

        public static ExecuteCommandSyntax At(this ExecuteCommandSyntax source, Target target)
        {
            return source.At().SetTarget(target);
        }

        public static ExecuteCommandSyntax At(this ExecuteCommandSyntax source, string target)
        {
            return source.At().SetTarget(target);
        }
        #endregion

        #region In
        public static ExecuteCommandSyntax InOverworld(this ExecuteCommandSyntax source)
        {
            return source.In().Overworld();
        }

        public static ExecuteCommandSyntax InNether(this ExecuteCommandSyntax source)
        {
            return source.In().Nether();
        }

        public static ExecuteCommandSyntax InTheEnd(this ExecuteCommandSyntax source)
        {
            return source.In().TheEnd();
        }

        public static ExecuteCommandSyntax In(this ExecuteCommandSyntax source, string dimension)
        {
            return source.In().SetDimension(dimension);
        }
        #endregion

        #region RotatedAs
        public static ExecuteCommandSyntax RotatedAs(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.RotatedAs().SetTarget(selector);
        }

        public static ExecuteCommandSyntax RotatedAs(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.RotatedAs().SetTarget(guid);
        }

        public static ExecuteCommandSyntax RotatedAs(this ExecuteCommandSyntax source, Target target)
        {
            return source.RotatedAs().SetTarget(target);
        }

        public static ExecuteCommandSyntax RotatedAs(this ExecuteCommandSyntax source, string target)
        {
            return source.RotatedAs().SetTarget(target);
        }
        #endregion

        #region FacingEntity
        public static ExecuteCommandSyntax FacingEntity(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.FacingEntity().SetTarget(selector);
        }

        public static ExecuteCommandSyntax FacingEntity(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.FacingEntity().SetTarget(guid);
        }

        public static ExecuteCommandSyntax FacingEntity(this ExecuteCommandSyntax source, Target target)
        {
            return source.FacingEntity().SetTarget(target);
        }

        public static ExecuteCommandSyntax FacingEntity(this ExecuteCommandSyntax source, string target)
        {
            return source.FacingEntity().SetTarget(target);
        }
        #endregion

        #region PositionedAs
        public static ExecuteCommandSyntax PositionedAs(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.PositionedAs().SetTarget(selector);
        }

        public static ExecuteCommandSyntax PositionedAs(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.PositionedAs().SetTarget(guid);
        }

        public static ExecuteCommandSyntax PositionedAs(this ExecuteCommandSyntax source, Target target)
        {
            return source.PositionedAs().SetTarget(target);
        }

        public static ExecuteCommandSyntax PositionedAs(this ExecuteCommandSyntax source, string target)
        {
            return source.PositionedAs().SetTarget(target);
        }
        #endregion

        #region Facing
        public static ExecuteCommandSyntax Facing(this ExecuteCommandSyntax source, double x, double y, double z)
        {
            return source.Facing().SetPosition(x, y, z);
        }

        public static ExecuteCommandSyntax Facing<T>(this ExecuteCommandSyntax source, T position) where T : IVector3<double>
        {
            return source.Facing().SetPosition(position);
        }
        #endregion

        #region Positioned
        public static ExecuteCommandSyntax Positioned(this ExecuteCommandSyntax source, double x, double y, double z)
        {
            return source.Positioned().SetPosition(x, y, z);
        }

        public static ExecuteCommandSyntax Positioned<T>(this ExecuteCommandSyntax source, T position) where T : IVector3<double>
        {
            return source.Positioned().SetPosition(position);
        }
        #endregion

        #region IfEntity
        public static ExecuteCommandSyntax IfEntity(this ExecuteCommandSyntax source, Selector selector)
        {
            return source.If().Entity().SetTarget(selector);
        }

        public static ExecuteCommandSyntax IfEntity(this ExecuteCommandSyntax source, Guid guid)
        {
            return source.If().Entity().SetTarget(guid);
        }

        public static ExecuteCommandSyntax IfEntity(this ExecuteCommandSyntax source, Target target)
        {
            return source.If().Entity().SetTarget(target);
        }

        public static ExecuteCommandSyntax IfEntity(this ExecuteCommandSyntax source, string target)
        {
            return source.If().Entity().SetTarget(target);
        }
        #endregion

        #region IfBlock
        public static ExecuteCommandSyntax IfBlock(this ExecuteCommandSyntax source, int x, int y, int z, string blockId)
        {
            return source.If().Block().SetPosition(x, y, z).SetBlock(blockId);
        }

        public static ExecuteCommandSyntax IfBlock<T>(this ExecuteCommandSyntax source, T position, string blockId) where T : IVector3<int>
        {
            return source.If().Block().SetPosition(position).SetBlock(blockId);
        }
        #endregion

        #region IfBiome
        public static ExecuteCommandSyntax IfBiome(this ExecuteCommandSyntax source, int x, int y, int z, string biomeId)
        {
            return source.If().Biome().SetPosition(x, y, z).SetBiome(biomeId);
        }

        public static ExecuteCommandSyntax IfBiome<T>(this ExecuteCommandSyntax source, T position, string biomeId) where T : IVector3<int>
        {
            return source.If().Biome().SetPosition(position).SetBiome(biomeId);
        }
        #endregion

        #region IfLoaded
        public static ExecuteCommandSyntax IfLoaded(this ExecuteCommandSyntax source, int x, int y, int z)
        {
            return source.If().Loaded().SetPosition(x, y, z);
        }

        public static ExecuteCommandSyntax IfLoaded<T>(this ExecuteCommandSyntax source, T position) where T : IVector3<int>
        {
            return source.If().Loaded().SetPosition(position);
        }
        #endregion

        #region IfDimension
        public static ExecuteCommandSyntax IfOverworld(this ExecuteCommandSyntax source)
        {
            return source.If().Dimension().Overworld();
        }

        public static ExecuteCommandSyntax IfNether(this ExecuteCommandSyntax source)
        {
            return source.If().Dimension().Nether();
        }

        public static ExecuteCommandSyntax IfTheEnd(this ExecuteCommandSyntax source)
        {
            return source.If().Dimension().TheEnd();
        }

        public static ExecuteCommandSyntax IfDimension(this ExecuteCommandSyntax source, string dimension)
        {
            return source.If().Dimension().SetDimension(dimension);
        }
        #endregion

        public static RunSyntax Run(this ExecuteCommandSyntax source, string command)
        {
            return source.Run().Command(command);
        }
    }
}
