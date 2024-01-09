using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Selectors
{
    public enum Target
    {
        /// <summary>
        /// 所有玩家
        /// </summary>
        AllPlayers,

        /// <summary>
        /// 所有实体
        /// </summary>
        AllEntities,

        /// <summary>
        /// 随机玩家
        /// </summary>
        RandomPlayer,

        /// <summary>
        /// 距离最近的玩家
        /// </summary>
        NearestPlayer,

        /// <summary>
        /// 命令执行者
        /// </summary>
        CommandExecutor
    }
}
