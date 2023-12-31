﻿using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public static class ResourcePackReader
    {
        public static ResourceEntryManager Load(params string[] packs)
        {
            ArgumentNullException.ThrowIfNull(packs, nameof(packs));

            ResourceEntryManager result = new();
            foreach (string pack in packs)
            {
                ZipPack zipPack = new(pack);
                result.ZipPacks.Add(zipPack);
                foreach (string modid in zipPack.GetDirectorys("assets"))
                {
                    ResourceEntry entry = new(modid);

                    foreach (var blockState in zipPack.GetFiles(entry.Path.BlockStates))
                        entry.BlockStates.Add(blockState.Name, blockState);

                    foreach (var blockModel in zipPack.GetFiles(entry.Path.BlockModels))
                        entry.BlockModels.Add(blockModel.Name, blockModel);

                    foreach (var blockTexture in zipPack.GetFiles(entry.Path.BlockTextures))
                        entry.BlockTextures.Add(blockTexture.Name, blockTexture);

                    foreach (var itemModel in zipPack.GetFiles(entry.Path.ItemModels))
                        entry.ItemModels.Add(itemModel.Name, itemModel);

                    foreach (var itemTexture in zipPack.GetFiles(entry.Path.ItemTextures))
                        entry.ItemTextures.Add(itemTexture.Name, itemTexture);

                    foreach (var language in zipPack.GetFiles(entry.Path.Languages))
                        entry.Languages.Add(language.Name, language);

                    result.Overwrite(entry);
                }
            }
            return result;
        }
    }
}
