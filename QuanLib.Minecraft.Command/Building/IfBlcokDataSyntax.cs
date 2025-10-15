using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfBlcokDataSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private IVector3<int>? _position;
        private string? _nbtPath;

        public IfBlcokDataSyntax SetPosition<T>(T position) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            _position = position;
            return this;
        }

        public IfBlcokDataSyntax SetPosition(int x, int y, int z)
        {
            _position = new Vector3<int>(x, y, z);
            return this;
        }

        public IfBlcokDataSyntax SetNbtPath(string nbtPath)
        {
            ArgumentNullException.ThrowIfNull(nbtPath, nameof(nbtPath));

            _nbtPath = nbtPath;
            return this;
        }

        public ExecuteCommandSyntax EndIf()
        {
            if (_position == null)
                throw new InvalidOperationException("位置未设置");
            if (string.IsNullOrEmpty(_nbtPath))
                throw new InvalidOperationException("NBT路径未设置");

            SetSyntax($"{_position.X} {_position.Y} {_position.Z} {_nbtPath}");
            return new ExecuteCommandSyntax(this);
        }
    }
}
