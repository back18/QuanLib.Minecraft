using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class OnCommandSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        /// <summary>
        /// 最近5秒内对当前执行者造成最后伤害的实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Attacker()
        {
            SetSyntax("attacker");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 控制当前执行者的实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Controller()
        {
            SetSyntax("controller");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 用拴绳牵引当前执行者的实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Leasher()
        {
            SetSyntax("leasher");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 当前执行者的来源。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Origin()
        {
            SetSyntax("origin");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 当前执行者为可驯服生物时，此实体的主人。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Owner()
        {
            SetSyntax("owner");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 直接骑乘于当前执行者的实体。若有多个骑乘于当前执行者的实体，则实体选择顺序和骑乘先后顺序相同。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Passengers()
        {
            SetSyntax("passengers");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 获取带目标实体的目标。目前，所有AI生物和交互实体均属于带目标实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Target()
        {
            SetSyntax("target");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 正在被当前执行者骑乘的实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> Vehicle()
        {
            SetSyntax("vehicle");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }
    }
}
