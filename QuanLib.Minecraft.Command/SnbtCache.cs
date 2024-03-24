using QuanLib.Core;
using QuanLib.Game;
using QuanLib.Minecraft.NBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command
{
    public class SnbtCache
    {
        public SnbtCache(TimeSpan expirationTime)
        {
            Position = new(expirationTime);
            Rotation = new(expirationTime);
            Uuid = new(expirationTime);
            Health = new(expirationTime);
            SelectedItemSlot = new(expirationTime);
            SelectedItem = new(expirationTime);
            DualWieldItem = new(expirationTime);
        }

        public CacheDictionary<string, Vector3<double>> Position { get; }

        public CacheDictionary<string, Rotation> Rotation { get; }

        public CacheDictionary<string, Guid> Uuid { get; }

        public CacheDictionary<string, float> Health { get; }

        public CacheDictionary<string, int> SelectedItemSlot { get; }

        public CacheDictionary<string, Item> SelectedItem { get; }

        public CacheDictionary<string, Item> DualWieldItem { get; }

        public void Clear()
        {
            Position.Clear();
            Rotation.Clear();
            Uuid.Clear();
            Health.Clear();
            SelectedItemSlot.Clear();
            SelectedItem.Clear();
            DualWieldItem.Clear();
        }
    }
}
