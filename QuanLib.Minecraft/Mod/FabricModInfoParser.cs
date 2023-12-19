using Newtonsoft.Json;
using QuanLib.Core.Extensions;
using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class FabricModInfoParser : ModInfoParser
    {
        public override string ModInfoPath => "fabric.mod.json";

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
                FabricModInfo.Model? model = JsonConvert.DeserializeObject<FabricModInfo.Model>(stream.ToUtf8Text());
                if (model is null)
                    goto fail;

                result = new FabricModInfo(model);
                return true;
            }
            catch (Exception e)
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
