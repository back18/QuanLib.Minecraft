using QuanLib.Core;
using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT.Models
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

        public short SleepTimer { get; }

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

        public new class Model : MobEntity.Model
        {
            [Nullable]
            public object? abilities { get; set; }

            public required int DataVersion { get; set; }

            public required string Dimension { get; set; }

            public required object[] EnderItems { get; set; }

            [Nullable]
            public object? EnteredNetherPosition { get; set; }

            public required float foodExhaustionLevel { get; set; }

            public required int foodLevel { get; set; }

            public required float foodSaturationLevel { get; set; }

            public required int foodTickTimer { get; set; }

            public required object[] Inventory { get; set; }

            public required object LastDeathLocation { get; set; }

            public required int playerGameType { get; set; }

            public required int previousPlayerGameType { get; set; }

            public required object recipeBook { get; set; }

            [Nullable]
            public object? RootVehicle { get; set; }

            public required int Score { get; set; }

            public required bool seenCredits { get; set; }

            public required int SelectedItemSlot { get; set; }

            public required short SleepTimer { get; set; }

            public required Entity ShoulderEntityLeft { get; set; }

            public required Entity ShoulderEntityRight { get; set; }

            public required float SpawnAngle { get; set; }

            [Nullable]
            public string? SpawnDimension { get; set; }

            [Nullable]
            public bool? SpawnForced { get; set; }

            public required int SpawnX { get; set; }

            public required int SpawnY { get; set; }

            public required int SpawnZ { get; set; }

            public required object warden_spawn_tracker { get; set; }

            public required int XpLevel { get; set; }

            public required float XpP { get; set; }

            public required int XpSeed { get; set; }

            public required int XpTotal { get; set; }
        }
    }
}
