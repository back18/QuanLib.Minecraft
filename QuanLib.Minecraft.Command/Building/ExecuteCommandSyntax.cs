using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class ExecuteCommandSyntax(ICommandSyntax? previous) : CommandSyntax(previous), ICreatible<ExecuteCommandSyntax>
    {
        /// <summary>
        /// 将执行者设置为特定实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> As()
        {
            SetSyntax("as");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将执行位置、执行朝向和执行维度设置为指定实体的坐标、朝向和维度。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> At()
        {
            SetSyntax("at");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将命令执行维度设置为特定维度。
        /// </summary>
        /// <returns></returns>
        public DimensionSyntax In()
        {
            SetSyntax("in");
            return new DimensionSyntax(this);
        }

        /// <summary>
        /// 将执行者设置为与当前执行者有指定类型的关系的实体。
        /// </summary>
        /// <returns></returns>
        public OnCommandSyntax On()
        {
            SetSyntax("on");
            return new OnCommandSyntax(this);
        }

        /// <summary>
        /// 将执行基准点设置为实体的脚部。
        /// </summary>
        /// <returns></returns>
        public ExecuteCommandSyntax AnchoredFeet()
        {
            SetSyntax("anchored feet");
            return new ExecuteCommandSyntax(this);
        }

        /// <summary>
        /// 将执行基准点设置为实体的眼部。
        /// </summary>
        /// <returns></returns>
        public ExecuteCommandSyntax AnchoredEyes()
        {
            SetSyntax("anchored eyes");
            return new ExecuteCommandSyntax(this);
        }

        /// <summary>
        /// 将执行朝向设为特定方向。
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public ExecuteCommandSyntax Rotated(double yaw, double pitch)
        {
            SetSyntax($"rotated {yaw} {pitch}");
            return new ExecuteCommandSyntax(this);
        }

        /// <summary>
        /// 将执行朝向设为指定实体的朝向。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> RotatedAs()
        {
            SetSyntax("rotated as");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将命令的执行朝向设置为面向指定坐标。
        /// </summary>
        /// <returns></returns>
        public EntityPositionSyntax<ExecuteCommandSyntax> Facing()
        {
            SetSyntax("facing");
            return new EntityPositionSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将命令的执行朝向设置为面向指定实体。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> FacingEntity()
        {
            SetSyntax("facing entity");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将执行位置设置为指定坐标。
        /// </summary>
        /// <returns></returns>
        public EntityPositionSyntax<ExecuteCommandSyntax> Positioned()
        {
            SetSyntax("positioned");
            return new EntityPositionSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将执行位置设置为指定实体位置。
        /// </summary>
        /// <returns></returns>
        public SelectorSyntax<ExecuteCommandSyntax> PositionedAs()
        {
            SetSyntax("positioned as");
            return new SelectorSyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 将执行位置设置为符合特定高度图的一纵列方块的最高的位置。
        /// </summary>
        /// <returns></returns>
        public PositionedOverSyntax PositionedOver()
        {
            SetSyntax("positioned over");
            return new PositionedOverSyntax(this);
        }

        /// <summary>
        /// 将执行位置转换为方块坐标。
        /// </summary>
        /// <returns></returns>
        public AlignSyntax Align()
        {
            SetSyntax("align");
            return new AlignSyntax(this);
        }

        /// <summary>
        /// 立即创建一个实体，并将此实体设置为执行者。
        /// </summary>
        /// <returns></returns>
        public EntitySyntax<ExecuteCommandSyntax> Summon()
        {
            SetSyntax($"summon");
            return new EntitySyntax<ExecuteCommandSyntax>(this);
        }

        /// <summary>
        /// 条件子命令，if表示“如果……就”
        /// </summary>
        /// <returns></returns>
        public IfCommandSyntax If()
        {
            SetSyntax("if");
            return new IfCommandSyntax(this);
        }

        /// <summary>
        /// 条件子命令，unless表示“除非……否则”
        /// </summary>
        /// <returns></returns>
        public IfCommandSyntax Unless()
        {
            SetSyntax("unless");
            return new IfCommandSyntax(this);
        }

        public RunSyntax Run()
        {
            SetSyntax("run");
            return new RunSyntax(this);
        }

        public string Build()
        {
            List<string> parts = [];
            ICommandSyntax? current = Previous;
            while (current is not null)
            {
                parts.Add(current.GetSyntax());
                current = current.Previous;
            }
            parts.Reverse();
            return string.Join(' ', parts);
        }

        public static ExecuteCommandSyntax Create(ICommandSyntax? previous)
        {
            return new ExecuteCommandSyntax(previous);
        }
    }
}
