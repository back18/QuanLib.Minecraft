using Nett;
using QuanLib.Core.Extensions;
using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class ForgeModInfoParser : ModInfoParser
    {
        public override string ModInfoPath => "META-INF/mods.toml";

        public override bool TryParse(ZipPack zipPack, [MaybeNullWhen(false)] out ModInfo result)
        {
            if (zipPack is null)
                goto fail;

            Stream? stream = null;
            try
            {
                if (!zipPack.TryGetValue(ModInfoPath, out var entry))
                    goto fail;

                stream = entry.Open();
                TomlTable table = Toml.ReadStream(stream);
                ForgeModInfo.Model model = table.Get<ForgeModInfo.Model>();
                ForgeModInfo.ModModel? modModel = model.mods?.FirstOrDefault();
                if (modModel is null)
                    goto fail;

                if (modModel.version == "${file.jarVersion}")
                {
                    if (zipPack.TryGetValue("META-INF/MANIFEST.MF", out var manifestEntry))
                    {
                        Stream manifestStream = manifestEntry.Open();
                        string manifestText = manifestStream.ToUtf8Text();
                        Dictionary<string, string> imanifest = ManifestParser.Parse(manifestText);
                        if (imanifest.TryGetValue("Implementation-Version", out var version))
                            modModel.version = version;
                    }
                }

                result = new ForgeModInfo(model);
                return true;
            }
            catch
            {
                goto fail;
            }
            finally
            {
                stream?.Dispose();
            }

            fail:
            result = null;
            return false;
        }
    }
}
