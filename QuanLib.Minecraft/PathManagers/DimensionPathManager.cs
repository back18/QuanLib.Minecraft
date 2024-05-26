using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class DimensionPathManager
    {
        public DimensionPathManager(string dimensionDirectory)
        {
            ArgumentException.ThrowIfNullOrEmpty(dimensionDirectory, nameof(dimensionDirectory));

            _dimensionPaths = new(dimensionDirectory);
        }

        protected readonly DimensionPaths _dimensionPaths;

        public DirectoryInfo Dimension => _dimensionPaths.Dimension.CreateDirectoryInfo();

        public DirectoryInfo Dimension_Data => _dimensionPaths.Dimension_Data.CreateDirectoryInfo();

        public DirectoryInfo Dimension_Region => _dimensionPaths.Dimension_Region.CreateDirectoryInfo();

        public DirectoryInfo Dimension_Entities => _dimensionPaths.Dimension_Entities.CreateDirectoryInfo();

        public DirectoryInfo Dimension_Poi => _dimensionPaths.Dimension_Poi.CreateDirectoryInfo();

        protected class DimensionPaths
        {
            public DimensionPaths(string dimensionDirectory)
            {
                ArgumentException.ThrowIfNullOrEmpty(dimensionDirectory, nameof(dimensionDirectory));

                Dimension = dimensionDirectory;
                Dimension_Data = Dimension.PathCombine("data");
                Dimension_Region = Dimension.PathCombine("region");
                Dimension_Entities = Dimension.PathCombine("entities");
                Dimension_Poi = Dimension.PathCombine("poi");
            }

            public readonly string Dimension;

            public readonly string Dimension_Data;

            public readonly string Dimension_Region;

            public readonly string Dimension_Entities;

            public readonly string Dimension_Poi;
        }
    }
}
