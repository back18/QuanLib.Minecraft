using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class WorldPathManager : DimensionPathManager
    {
        public WorldPathManager(string dimensionDirectory) : base(dimensionDirectory)
        {
            _worldPaths = new(dimensionDirectory);
        }

        protected readonly WorldPaths _worldPaths;

        public DirectoryInfo World => _worldPaths.World.CreateDirectoryInfo();

        public DirectoryInfo World_DataPacks => _worldPaths.World_DataPacks.CreateDirectoryInfo();

        public DirectoryInfo World_Stats => _worldPaths.World_Stats.CreateDirectoryInfo();

        public DirectoryInfo World_Advancements => _worldPaths.World_Advancements.CreateDirectoryInfo();

        public DirectoryInfo World_TheNether => _worldPaths.World_TheNether.CreateDirectoryInfo();

        public DirectoryInfo World_TheEnd => _worldPaths.World_TheEnd.CreateDirectoryInfo();

        public FileInfo World_Icon => _worldPaths.World_Icon.CreateFileInfo();

        public FileInfo World_LevelDet => _worldPaths.World_LevelDet.CreateFileInfo();

        public FileInfo World_LevelDetOld => _worldPaths.World_LevelDetOld.CreateFileInfo();

        public FileInfo World_SessionLock => _worldPaths.World_SessionLock.CreateFileInfo();

        protected class WorldPaths : DimensionPaths
        {
            public WorldPaths(string dimensionDirectory) : base(dimensionDirectory)
            {
                World = dimensionDirectory;
                World_DataPacks = World.PathCombine("datapacks");
                World_Stats = World.PathCombine("stats");
                World_Advancements = World.PathCombine("advancements");
                World_TheNether = World.PathCombine("DIM-1");
                World_TheEnd = World.PathCombine("DIM1");
                World_Icon = World.PathCombine("icon.png");
                World_LevelDet = World.PathCombine("level.dat");
                World_LevelDetOld = World.PathCombine("level.dat_old");
                World_SessionLock = World.PathCombine("session.lock");
            }

            public readonly string World;

            public readonly string World_DataPacks;

            public readonly string World_Stats;

            public readonly string World_Advancements;

            public readonly string World_TheNether;

            public readonly string World_TheEnd;

            public readonly string World_Icon;

            public readonly string World_LevelDet;

            public readonly string World_LevelDetOld;

            public readonly string World_SessionLock;
        }
    }
}
