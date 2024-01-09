using QuanLib.Core;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.SNBT.Models
{
    public class PlayerEntity : MobEntity
    {
        public PlayerEntity(Model model) : base(model)
        {
            Abilities = model.abilities;
            DataVersion = model.DataVersion;
            Dimension = model.Dimension;
            EnderItems = new(model.EnderItems);
            EnteredNetherPosition = model.EnteredNetherPosition;
            FoodExhaustionLevel = model.foodExhaustionLevel;
            FoodLevel = model.foodLevel;
            FoodSaturationLevel = model.foodSaturationLevel;
            FoodTickTimer = model.foodTickTimer;
            Inventory = new(model.Inventory);
            LastDeathLocation = model.LastDeathLocation;
            PlayerGameType = model.playerGameType;
            PreviousPlayerGameType = model.previousPlayerGameType;
            RecipeBook = model.recipeBook;
            RootVehicle = model.RootVehicle;
            Score = model.Score;
            SeenCredits = model.seenCredits;
            SelectedItemSlot = model.SelectedItemSlot;
            SleepTimer = model.SleepTimer;
            ShoulderEntityLeft = model.ShoulderEntityLeft;
            ShoulderEntityRight = model.ShoulderEntityRight;
            SpawnAngle = model.SpawnAngle;
            SpawnDimension = model.SpawnDimension;
            SpawnForced = model.SpawnForced;
            Spawn = new(model.SpawnX, model.SpawnY, model.SpawnZ);
            WardenSpawnTracker = model.warden_spawn_tracker;
            XpLevel = model.XpLevel;
            XpP = model.XpP;
            XpSeed = model.XpSeed;
            XpTotal = model.XpTotal;
        }

        public object? Abilities { get; }

        public int DataVersion { get; }

        public string Dimension { get; }

        public ReadOnlyCollection<object> EnderItems { get; }

        public object? EnteredNetherPosition { get; }

        public float FoodExhaustionLevel { get; }

        public int FoodLevel { get; }

        public float FoodSaturationLevel { get; }

        public int FoodTickTimer { get; }

        public ReadOnlyCollection<object> Inventory { get; }

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

        public BlockPos? Spawn { get; }

        public object WardenSpawnTracker { get; }

        public int XpLevel { get; }

        public float XpP { get; }

        public int XpSeed { get; }

        public int XpTotal { get; }

        public new class Model : MobEntity.Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            [Nullable]
            public object? abilities { get; set; }

            public int DataVersion { get; set; }

            public string Dimension { get; set; }

            public object[] EnderItems { get; set; }

            [Nullable]
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

            [Nullable]
            public object? RootVehicle { get; set; }

            public int Score { get; set; }

            public bool seenCredits { get; set; }

            public int SelectedItemSlot { get; set; }

            public int SleepTimer { get; set; }

            public Entity ShoulderEntityLeft { get; set; }

            public Entity ShoulderEntityRight { get; set; }

            public float SpawnAngle { get; set; }

            [Nullable]
            public string? SpawnDimension { get; set; }

            [Nullable]
            public bool? SpawnForced { get; set; }

            public int SpawnX { get; set; }

            public int SpawnY { get; set; }

            public int SpawnZ { get; set; }

            public object warden_spawn_tracker { get; set; }

            public int XpLevel { get; set; }

            public float XpP { get; set; }

            public int XpSeed { get; set; }

            public int XpTotal { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
