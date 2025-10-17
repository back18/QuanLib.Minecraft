using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public abstract class MinecraftVersion : IEquatable<MinecraftVersion>, IComparable<MinecraftVersion>
    {
        protected MinecraftVersion(string versionNumber, VersionType type, DateTime releaseTime)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionNumber);

            VersionNumber = versionNumber;
            Type = type;
            ReleaseTime = releaseTime;
        }

        public string VersionNumber { get; }

        public VersionType Type { get; }

        public DateTime ReleaseTime { get; }

        public abstract bool IsReleaseVersion {  get; }

        public abstract bool IsSnapshotVersion { get; }

        public abstract bool IsAncientVersion { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MinecraftVersion);
        }

        public bool Equals(MinecraftVersion? other)
        {
            if (other is null)
                return false;

            return VersionNumber == other.VersionNumber && Type == other.Type && ReleaseTime == other.ReleaseTime;
        }

        public virtual int CompareTo(MinecraftVersion? other)
        {
            if (other is null)
                return 1;

            return ReleaseTime.CompareTo(other.ReleaseTime);
        }

        public override string ToString()
        {
            return VersionNumber;
        }

        public override int GetHashCode()
        {
            return VersionNumber.GetHashCode();
        }

        public static bool operator ==(MinecraftVersion? left, MinecraftVersion? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(MinecraftVersion? left, MinecraftVersion? right)
        {
            return !(left == right);
        }

        public static bool operator <(MinecraftVersion? left, MinecraftVersion? right)
        {
            if (left is null || right is null)
                return false;

            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(MinecraftVersion? left, MinecraftVersion? right)
        {
            if (left is null || right is null)
                return false;

            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(MinecraftVersion? left, MinecraftVersion? right)
        {
            if (left is null || right is null)
                return false;

            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(MinecraftVersion? left, MinecraftVersion? right)
        {
            if (left is null || right is null)
                return false;

            return left.CompareTo(right) >= 0;
        }

        public class Model
        {
            public required string Version {  get; set; }

            public required string Type { get; set; }

            public required long ReleaseTime { get; set; }
        }
    }
}
