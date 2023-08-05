using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Data
{
    public class PlayerEntity : MobEntity
    {
        public PlayerEntity(Nbt nbt) : base(nbt)
        {
            Abilities = nbt.abilities;
            DataVersion = nbt.DataVersion;
            Dimension = nbt.Dimension;
            EnderItems = nbt.EnderItems;
            EnteredNetherPosition = nbt.EnteredNetherPosition;
            FoodExhaustionLevel = nbt.foodExhaustionLevel;
            FoodLevel = nbt.foodLevel;
            FoodSaturationLevel = nbt.foodSaturationLevel;
            FoodTickTimer = nbt.foodTickTimer;
            Inventory = nbt.Inventory;
            LastDeathLocation = nbt.LastDeathLocation;
            PlayerGameType = nbt.playerGameType;
            PreviousPlayerGameType = nbt.previousPlayerGameType;
            RecipeBook = nbt.recipeBook;
            RootVehicle = nbt.RootVehicle;
            Score = nbt.Score;
            SeenCredits = nbt.seenCredits;
            SelectedItemSlot = nbt.SelectedItemSlot;
            SleepTimer = nbt.SleepTimer;
            ShoulderEntityLeft = nbt.ShoulderEntityLeft;
            ShoulderEntityRight = nbt.ShoulderEntityRight;
            SpawnAngle = nbt.SpawnAngle;
            SpawnDimension = nbt.SpawnDimension;
            SpawnForced = nbt.SpawnForced;
            Spawn = new(nbt.SpawnX, nbt.SpawnY, nbt.SpawnZ);
            WardenSpawnTracker = nbt.warden_spawn_tracker;
            XpLevel = nbt.XpLevel;
            XpP = nbt.XpP;
            XpSeed = nbt.XpSeed;
            XpTotal = nbt.XpTotal;
        }

        public object? Abilities { get; }

        public int DataVersion { get; }

        public string Dimension { get; }

        public IReadOnlyList<object> EnderItems { get; }

        public object? EnteredNetherPosition { get; }

        public float FoodExhaustionLevel { get; }

        public int FoodLevel { get; }

        public float FoodSaturationLevel { get; }

        public int FoodTickTimer { get; }

        public IReadOnlyList<object> Inventory { get; }

        public object LastDeathLocation { get; }

        public int PlayerGameType { get; }

        public int PreviousPlayerGameType { get; }

        public object RecipeBook { get; }

        public object? RootVehicle { get; }

        public int Score { get; }

        public bool SeenCredits { get; }

        public int SelectedItemSlot { get; }

        public int SleepTimer { get; }

        public Entity ShoulderEntityLeft { get; }

        public Entity ShoulderEntityRight { get; }

        public float SpawnAngle { get; }

        public string? SpawnDimension { get; }

        public bool? SpawnForced { get; }

        public Vector3<int>? Spawn { get; }

        public object WardenSpawnTracker { get; }

        public int XpLevel { get; }

        public float XpP { get; }

        public int XpSeed { get; }

        public int XpTotal { get; }

        public new class Nbt : MobEntity.Nbt
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            public object? abilities { get; set; }

            public int DataVersion { get; set; }

            public string Dimension { get; set; }

            public object[] EnderItems { get; set; }

            public object? EnteredNetherPosition { get; set; }

            public float foodExhaustionLevel { get; set; }

            public int foodLevel { get; set; }

            public float foodSaturationLevel { get; set; }

            public int foodTickTimer { get; set; }

            public object[] Inventory { get; set; }

            public object LastDeathLocation { get; set; }

            public int playerGameType { get; set; }

            public int previousPlayerGameType { get; set; }

            public object recipeBook { get; set; }

            public object? RootVehicle { get; set; }

            public int Score { get; set; }

            public bool seenCredits { get; set; }

            public int SelectedItemSlot { get; set; }

            public int SleepTimer { get; set; }

            public Entity ShoulderEntityLeft { get; set; }

            public Entity ShoulderEntityRight { get; set; }

            public float SpawnAngle { get; set; }

            public string? SpawnDimension { get; set; }

            public bool? SpawnForced { get; set; }

            public int SpawnX { get; set; }

            public int SpawnY { get; set; }

            public int SpawnZ { get; set; }

            public object warden_spawn_tracker { get; set; }

            public int XpLevel { get; set; }

            public float XpP { get; set; }

            public int XpSeed { get; set; }

            public int XpTotal { get; set; }
        }
    }
}
