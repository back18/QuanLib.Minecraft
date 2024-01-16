using QuanLib.IO.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public static class ResourcePackReader
    {
        public static ResourceEntryManager Load(params ZipPack[] zipPacks)
        {
            ArgumentNullException.ThrowIfNull(zipPacks, nameof(zipPacks));

            ResourceEntryManager result = new();
            foreach (var zipPack in zipPacks)
            {
                result.ZipPacks.Add(zipPack);
                foreach (string directory in zipPack.GetDirectoryPaths("assets"))
                {
                    ResourceEntry entry = new(Path.GetFileName(directory));

                    foreach (var file in zipPack.GetFilePaths(entry.Path.BlockStates))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.BlockStates.Add(zipItem.Name, zipItem);
                    }

                    foreach (var file in zipPack.GetFilePaths(entry.Path.BlockModels))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.BlockModels.Add(zipItem.Name, zipItem);
                    }

                    foreach (var file in zipPack.GetFilePaths(entry.Path.BlockTextures))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.BlockTextures.Add(zipItem.Name, zipItem);
                    }

                    foreach (var file in zipPack.GetFilePaths(entry.Path.ItemModels))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.ItemModels.Add(zipItem.Name, zipItem);
                    }

                    foreach (var file in zipPack.GetFilePaths(entry.Path.ItemTextures))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.ItemTextures.Add(zipItem.Name, zipItem);
                    }

                    foreach (var file in zipPack.GetFilePaths(entry.Path.Languages))
                    {
                        ZipItem zipItem = zipPack.GetFile(file);
                        entry.Languages.Add(zipItem.Name, zipItem);
                    }

                    result.Overwrite(entry);
                }
            }

            return result;
        }

        public static ResourceEntryManager Load(params string[] zipPackPaths)
        {
            ArgumentNullException.ThrowIfNull(zipPackPaths, nameof(zipPackPaths));

            List<ZipPack> zipPacks = [];
            foreach (var zipPackPath in zipPackPaths)
            {
                ZipPack zipPack = new(zipPackPath);
                zipPacks.Add(zipPack);
            }

            return Load(zipPackPaths.ToArray());
        }
    }
}
