using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command
{
    public readonly struct ConditionalResult(bool isSuccess, int count)
    {
        public readonly bool IsSuccess = isSuccess;

        public readonly int Count = count;
    }
}
