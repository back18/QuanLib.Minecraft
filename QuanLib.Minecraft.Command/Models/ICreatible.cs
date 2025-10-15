using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public interface ICreatible<TSelf> where TSelf : ICreatible<TSelf>
    {
        static abstract TSelf Create(LanguageManager languageManager);
    }
}
