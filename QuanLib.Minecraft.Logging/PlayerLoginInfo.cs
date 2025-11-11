using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public class PlayerLoginInfo
    {
        public PlayerLoginInfo(string name, string address, int id, Vector3<double> position)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(address, nameof(address));

            Name = name;
            Address = address;
            Id = id;
            Position = position;
        }

        public string Name { get; }

        public string Address { get; }

        public int Id { get; }

        public Vector3<double> Position { get; }

        public override string ToString()
        {
            return $"{Name}[{Address}] logged in with entity id {Id} at ({Position.X}, {Position.Y}, {Position.Z})";
        }
    }
}
