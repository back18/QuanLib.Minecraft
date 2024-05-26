using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class ResourcePackPathManager
    {
        public ResourcePackPathManager(string assetDirectory)
        {
            ArgumentException.ThrowIfNullOrEmpty(assetDirectory, nameof(assetDirectory));

            _resourcePackPaths = new(assetDirectory);
        }

        protected readonly ResourcePackPaths _resourcePackPaths;

        public DirectoryInfo Asset => _resourcePackPaths.Asset.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures => _resourcePackPaths.Asset_Textures.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Block => _resourcePackPaths.Asset_Textures_Block.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Entity => _resourcePackPaths.Asset_Textures_Entity.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Item => _resourcePackPaths.Asset_Textures_Item.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Models => _resourcePackPaths.Asset_Textures_Models.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Painting => _resourcePackPaths.Asset_Textures_Painting.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Map => _resourcePackPaths.Asset_Textures_Map.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Effect => _resourcePackPaths.Asset_Textures_Effect.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_MobEffect => _resourcePackPaths.Asset_Textures_MobEffect.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Environment => _resourcePackPaths.Asset_Textures_Environment.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Misc => _resourcePackPaths.Asset_Textures_Misc.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Gui => _resourcePackPaths.Asset_Textures_Gui.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Font => _resourcePackPaths.Asset_Textures_Font.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Particle => _resourcePackPaths.Asset_Textures_Particle.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Trims => _resourcePackPaths.Asset_Textures_Trims.CreateDirectoryInfo();

        public DirectoryInfo Asset_Textures_Colormap => _resourcePackPaths.Asset_Textures_Colormap.CreateDirectoryInfo();

        public DirectoryInfo Asset_Models => _resourcePackPaths.Asset_Models.CreateDirectoryInfo();

        public DirectoryInfo Asset_Models_Block => _resourcePackPaths.Asset_Models_Block.CreateDirectoryInfo();

        public DirectoryInfo Asset_Models_Item => _resourcePackPaths.Asset_Models_Item.CreateDirectoryInfo();

        public DirectoryInfo Asset_Particles => _resourcePackPaths.Asset_Particles.CreateDirectoryInfo();

        public DirectoryInfo Asset_BlockStates => _resourcePackPaths.Asset_BlockStates.CreateDirectoryInfo();

        public DirectoryInfo Asset_Font => _resourcePackPaths.Asset_Font.CreateDirectoryInfo();

        public DirectoryInfo Asset_Lang => _resourcePackPaths.Asset_Lang.CreateDirectoryInfo();

        public DirectoryInfo Asset_Texts => _resourcePackPaths.Asset_Texts.CreateDirectoryInfo();

        public DirectoryInfo Asset_Atlases => _resourcePackPaths.Asset_Atlases.CreateDirectoryInfo();

        public DirectoryInfo Asset_Shaders => _resourcePackPaths.Asset_Shaders.CreateDirectoryInfo();

        protected class ResourcePackPaths
        {
            public ResourcePackPaths(string assetDirectory)
            {
                ArgumentException.ThrowIfNullOrEmpty(assetDirectory, nameof(assetDirectory));

                Asset = assetDirectory;
                Asset_Textures = Asset.PathCombine("textures");
                Asset_Textures_Block = Asset_Textures.PathCombine("block");
                Asset_Textures_Entity = Asset_Textures.PathCombine("entity");
                Asset_Textures_Item = Asset_Textures.PathCombine("item");
                Asset_Textures_Models = Asset_Textures.PathCombine("models");
                Asset_Textures_Painting = Asset_Textures.PathCombine("painting");
                Asset_Textures_Map = Asset_Textures.PathCombine("map");
                Asset_Textures_Effect = Asset_Textures.PathCombine("effect");
                Asset_Textures_MobEffect = Asset_Textures.PathCombine("mob_effect");
                Asset_Textures_Environment = Asset_Textures.PathCombine("environment");
                Asset_Textures_Misc = Asset_Textures.PathCombine("misc");
                Asset_Textures_Gui = Asset_Textures.PathCombine("gui");
                Asset_Textures_Font = Asset_Textures.PathCombine("font");
                Asset_Textures_Particle = Asset_Textures.PathCombine("particle");
                Asset_Textures_Trims = Asset_Textures.PathCombine("trims");
                Asset_Textures_Colormap = Asset_Textures.PathCombine("colormap");
                Asset_Models = Asset.PathCombine("models");
                Asset_Models_Block = Asset_Models.PathCombine("block");
                Asset_Models_Item = Asset_Models.PathCombine("item");
                Asset_Particles = Asset.PathCombine("particles");
                Asset_BlockStates = Asset.PathCombine("blockstates");
                Asset_Font = Asset.PathCombine("font");
                Asset_Lang = Asset.PathCombine("lang");
                Asset_Texts = Asset.PathCombine("texts");
                Asset_Atlases = Asset.PathCombine("atlases");
                Asset_Shaders = Asset.PathCombine("shaders");
            }

            public readonly string Asset;

            public readonly string Asset_Textures;

            public readonly string Asset_Textures_Block;

            public readonly string Asset_Textures_Entity;

            public readonly string Asset_Textures_Item;

            public readonly string Asset_Textures_Models;

            public readonly string Asset_Textures_Painting;

            public readonly string Asset_Textures_Map;

            public readonly string Asset_Textures_Effect;

            public readonly string Asset_Textures_MobEffect;

            public readonly string Asset_Textures_Environment;

            public readonly string Asset_Textures_Misc;

            public readonly string Asset_Textures_Gui;

            public readonly string Asset_Textures_Font;

            public readonly string Asset_Textures_Particle;

            public readonly string Asset_Textures_Trims;

            public readonly string Asset_Textures_Colormap;

            public readonly string Asset_Models;

            public readonly string Asset_Models_Block;

            public readonly string Asset_Models_Item;

            public readonly string Asset_Particles;

            public readonly string Asset_BlockStates;

            public readonly string Asset_Font;

            public readonly string Asset_Lang;

            public readonly string Asset_Texts;

            public readonly string Asset_Atlases;

            public readonly string Asset_Shaders;
        }
    }
}
