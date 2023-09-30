using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.Command;

namespace QuanLib.Minecraft.Selectors
{
    public class TargetSelector : Selector
    {
        public TargetSelector(Target target)
        {
            Target = target;
        }

        public Target Target { get; }

        public TargetVector3Argument<double>? StartPosition { get; set; }

        public TargetVector3Argument<double>? EndPositionOffset { get; set; }

        public TargetArgument<double>? Distance { get; set; }

        public TargetArgument<string>? Name { get; set; }

        public TargetArgument<string>? Team { get; set; }

        public TargetArgument<string>? Family { get; set; }

        public TargetArgument<string>? Predicate { get; set; }

        public TargetArgument<string>? Type { get; set; }

        public TargetArgument<double>? XRotation { get; set; }

        public TargetArgument<double>? YRotation { get; set; }

        public TargetArgument<int>? Level { get; set; }

        public TargetArgument<Gamemode>? Gamemode { get; set; }

        public TargetArgument<int>? Limit { get; set; }

        public TargetArgument<Sort>? Sort { get; set; }

        public override string ToString()
        {
            List<string> items = new();

            if (StartPosition is not null)
                items.Add(StartPosition.ToString(new("x", "y", "z")));

            if (EndPositionOffset is not null)
                items.Add(EndPositionOffset.ToString(new("dx", "dy", "dz")));

            if (Distance is not null)
                items.Add(Distance.ToString("distance"));

            if (Name is not null)
                items.Add(Name.ToString("name"));

            if (Team is not null)
                items.Add(Team.ToString("team"));

            if (Family is not null)
                items.Add(Family.ToString("family"));

            if (Predicate is not null)
                items.Add(Predicate.ToString("predicate"));

            if (Type is not null)
                items.Add(Type.ToString("type"));

            if (XRotation is not null)
                items.Add(XRotation.ToString("x_rotation"));

            if (YRotation is not null)
                items.Add(YRotation.ToString("y_rotation"));

            if (Level is not null)
                items.Add(Level.ToString("level"));

            if (Gamemode is not null)
                items.Add(Gamemode.ToString("gamemode"));

            if (Limit is not null)
                items.Add(Limit.ToString("limit"));

            if (Sort is not null)
                items.Add(Sort.ToString("sort"));

            string result = Target.ToCommandArgument();
            if (items.Count > 0)
                result += $"[{string.Join(',', items)}]";

            return result;
        }
    }
}
