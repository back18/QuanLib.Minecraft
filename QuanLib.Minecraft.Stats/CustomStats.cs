using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public class CustomStats : IReadOnlyDictionary<string, int>
    {
        static CustomStats()
        {
            HashSet<string> statsKeys = [];
            foreach (PropertyInfo propertyInfo in typeof(CustomStats).GetProperties())
            {
                if (Attribute.GetCustomAttribute(propertyInfo, typeof(CustomStatsKeyAttribute)) is CustomStatsKeyAttribute attribute)
                    statsKeys.Add(attribute.Key);
            }

            StatsKeys = statsKeys.AsReadOnly();
        }

        private CustomStats(IReadOnlyDictionary<string, int> properties)
        {
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));
            if (properties.Count != StatsKeys.Count)
                throw new ArgumentException($"The number of properties must be {StatsKeys.Count}.", nameof(properties));

            _properties = properties;
        }

        private readonly IReadOnlyDictionary<string, int> _properties;
        public static readonly ReadOnlySet<string> StatsKeys;

        public const string ANIMALS_BRED = "minecraft:animals_bred";
        public const string AVIATE_ONE_CM = "minecraft:aviate_one_cm";
        public const string BELL_RING = "minecraft:bell_ring";
        public const string BOAT_ONE_CM = "minecraft:boat_one_cm";
        public const string CLEAN_ARMOR = "minecraft:clean_armor";
        public const string CLEAN_BANNER = "minecraft:clean_banner";
        public const string CLEAN_SHULKER_BOX = "minecraft:clean_shulker_box";
        public const string CLIMB_ONE_CM = "minecraft:climb_one_cm";
        public const string CROUCH_ONE_CM = "minecraft:crouch_one_cm";
        public const string DAMAGE_ABSORBED = "minecraft:damage_absorbed";
        public const string DAMAGE_BLOCKED_BY_SHIELD = "minecraft:damage_blocked_by_shield";
        public const string DAMAGE_DEALT = "minecraft:damage_dealt";
        public const string DAMAGE_DEALT_ABSORBED = "minecraft:damage_dealt_absorbed";
        public const string DAMAGE_DEALT_RESISTED = "minecraft:damage_dealt_resisted";
        public const string DAMAGE_RESISTED = "minecraft:damage_resisted";
        public const string DAMAGE_TAKEN = "minecraft:damage_taken";
        public const string DEATHS = "minecraft:deaths";
        public const string DROP = "minecraft:drop";
        public const string EAT_CAKE_SLICE = "minecraft:eat_cake_slice";
        public const string ENCHANT_ITEM = "minecraft:enchant_item";
        public const string FALL_ONE_CM = "minecraft:fall_one_cm";
        public const string FILL_CAULDRON = "minecraft:fill_cauldron";
        public const string FISH_CAUGHT = "minecraft:fish_caught";
        public const string FLY_ONE_CM = "minecraft:fly_one_cm";
        public const string HAPPY_GHAST_ONE_CM = "minecraft:happy_ghast_one_cm";
        public const string HORSE_ONE_CM = "minecraft:horse_one_cm";
        public const string INSPECT_DISPENSER = "minecraft:inspect_dispenser";
        public const string INSPECT_DROPPER = "minecraft:inspect_dropper";
        public const string INSPECT_HOPPER = "minecraft:inspect_hopper";
        public const string INTERACT_WITH_ANVIL = "minecraft:interact_with_anvil";
        public const string INTERACT_WITH_BEACON = "minecraft:interact_with_beacon";
        public const string INTERACT_WITH_BLAST_FURNACE = "minecraft:interact_with_blast_furnace";
        public const string INTERACT_WITH_BREWINGSTAND = "minecraft:interact_with_brewingstand";
        public const string INTERACT_WITH_CAMPFIRE = "minecraft:interact_with_campfire";
        public const string INTERACT_WITH_CARTOGRAPHY_TABLE = "minecraft:interact_with_cartography_table";
        public const string INTERACT_WITH_CRAFTING_TABLE = "minecraft:interact_with_crafting_table";
        public const string INTERACT_WITH_FURNACE = "minecraft:interact_with_furnace";
        public const string INTERACT_WITH_GRINDSTONE = "minecraft:interact_with_grindstone";
        public const string INTERACT_WITH_LECTERN = "minecraft:interact_with_lectern";
        public const string INTERACT_WITH_LOOM = "minecraft:interact_with_loom";
        public const string INTERACT_WITH_SMITHING_TABLE = "minecraft:interact_with_smithing_table";
        public const string INTERACT_WITH_SMOKER = "minecraft:interact_with_smoker";
        public const string INTERACT_WITH_STONECUTTER = "minecraft:interact_with_stonecutter";
        public const string JUMP = "minecraft:jump";
        public const string LEAVE_GAME = "minecraft:leave_game";
        public const string MINECART_ONE_CM = "minecraft:minecart_one_cm";
        public const string MOB_KILLS = "minecraft:mob_kills";
        public const string NAUTILUS_ONE_CM = "minecraft:nautilus_one_cm";
        public const string OPEN_BARREL = "minecraft:open_barrel";
        public const string OPEN_CHEST = "minecraft:open_chest";
        public const string OPEN_ENDERCHEST = "minecraft:open_enderchest";
        public const string OPEN_SHULKER_BOX = "minecraft:open_shulker_box";
        public const string PIG_ONE_CM = "minecraft:pig_one_cm";
        public const string PLAY_NOTEBLOCK = "minecraft:play_noteblock";
        public const string PLAY_RECORD = "minecraft:play_record";
        public const string PLAY_TIME = "minecraft:play_time";
        public const string PLAYER_KILLS = "minecraft:player_kills";
        public const string POT_FLOWER = "minecraft:pot_flower";
        public const string RAID_TRIGGER = "minecraft:raid_trigger";
        public const string RAID_WIN = "minecraft:raid_win";
        public const string SLEEP_IN_BED = "minecraft:sleep_in_bed";
        public const string SNEAK_TIME = "minecraft:sneak_time";
        public const string SPRINT_ONE_CM = "minecraft:sprint_one_cm";
        public const string STRIDER_ONE_CM = "minecraft:strider_one_cm";
        public const string SWIM_ONE_CM = "minecraft:swim_one_cm";
        public const string TALKED_TO_VILLAGER = "minecraft:talked_to_villager";
        public const string TARGET_HIT = "minecraft:target_hit";
        public const string TIME_SINCE_DEATH = "minecraft:time_since_death";
        public const string TIME_SINCE_REST = "minecraft:time_since_rest";
        public const string TOTAL_WORLD_TIME = "minecraft:total_world_time";
        public const string TRADED_WITH_VILLAGER = "minecraft:traded_with_villager";
        public const string TRIGGER_TRAPPED_CHEST = "minecraft:trigger_trapped_chest";
        public const string TUNE_NOTEBLOCK = "minecraft:tune_noteblock";
        public const string USE_CAULDRON = "minecraft:use_cauldron";
        public const string WALK_ON_WATER_ONE_CM = "minecraft:walk_on_water_one_cm";
        public const string WALK_ONE_CM = "minecraft:walk_one_cm";
        public const string WALK_UNDER_WATER_ONE_CM = "minecraft:walk_under_water_one_cm";

        /// <summary>
        /// 繁殖动物次数
        /// </summary>
        /// <remarks>
        /// 玩家通过给成对的动物喂食来使之繁殖的次数。
        /// </remarks>
        [CustomStatsKey(ANIMALS_BRED)]
        public int AnimalsBred => _properties[ANIMALS_BRED];

        /// <summary>
        /// 鞘翅滑行距离
        /// </summary>
        /// <remarks>
        /// 玩家使用鞘翅滑行的总距离。
        /// </remarks>
        [CustomStatsKey(AVIATE_ONE_CM)]
        public int AviateOneCm => _properties[AVIATE_ONE_CM];

        /// <summary>
        /// 鸣钟次数
        /// </summary>
        /// <remarks>
        /// 玩家敲响钟的次数。
        /// </remarks>
        [CustomStatsKey(BELL_RING)]
        public int BellRing => _properties[BELL_RING];

        /// <summary>
        /// 坐船移动距离
        /// </summary>
        /// <remarks>
        /// 玩家乘船移动的总距离。
        /// </remarks>
        [CustomStatsKey(BOAT_ONE_CM)]
        public int BoatOneCm => _properties[BOAT_ONE_CM];

        /// <summary>
        /// 清洗盔甲次数
        /// </summary>
        /// <remarks>
        /// 玩家使用炼药锅洗去皮革盔甲染色的次数。
        /// </remarks>
        [CustomStatsKey(CLEAN_ARMOR)]
        public int CleanArmor => _properties[CLEAN_ARMOR];

        /// <summary>
        /// 清洗旗帜次数
        /// </summary>
        /// <remarks>
        /// 玩家使用炼药锅洗去旗帜上的图案的次数。
        /// </remarks>
        [CustomStatsKey(CLEAN_BANNER)]
        public int CleanBanner => _properties[CLEAN_BANNER];

        /// <summary>
        /// 潜影盒清洗次数
        /// </summary>
        /// <remarks>
        /// 玩家使用炼药锅洗去潜影盒染色的次数。
        /// </remarks>
        [CustomStatsKey(CLEAN_SHULKER_BOX)]
        public int CleanShulkerBox => _properties[CLEAN_SHULKER_BOX];

        /// <summary>
        /// 攀爬距离
        /// </summary>
        /// <remarks>
        /// 玩家通过梯子或藤蔓向上移动的总距离。
        /// </remarks>
        [CustomStatsKey(CLIMB_ONE_CM)]
        public int ClimbOneCm => _properties[CLIMB_ONE_CM];

        /// <summary>
        /// 潜行距离
        /// </summary>
        /// <remarks>
        /// 玩家潜行时移动的总距离。
        /// </remarks>
        [CustomStatsKey(CROUCH_ONE_CM)]
        public int CrouchOneCm => _properties[CROUCH_ONE_CM];

        /// <summary>
        /// 吸收伤害量
        /// </summary>
        /// <remarks>
        /// 玩家吸收的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_ABSORBED)]
        public int DamageAbsorbed => _properties[DAMAGE_ABSORBED];

        /// <summary>
        /// 盾牌格挡伤害量
        /// </summary>
        /// <remarks>
        /// 玩家使用盾牌抵挡的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_BLOCKED_BY_SHIELD)]
        public int DamageBlockedByShield => _properties[DAMAGE_BLOCKED_BY_SHIELD];

        /// <summary>
        /// 造成伤害量
        /// </summary>
        /// <remarks>
        /// 玩家造成的伤害总量，单位为1（♥）的1⁄10，只统计近战攻击造成的伤害。
        /// </remarks>
        [CustomStatsKey(DAMAGE_DEALT)]
        public int DamageDealt => _properties[DAMAGE_DEALT];

        /// <summary>
        /// 造成伤害量（被吸收）
        /// </summary>
        /// <remarks>
        /// 玩家造成但被吸收的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_DEALT_ABSORBED)]
        public int DamageDealtAbsorbed => _properties[DAMAGE_DEALT_ABSORBED];

        /// <summary>
        /// 造成伤害量（被抵挡）
        /// </summary>
        /// <remarks>
        /// 玩家造成但被抵挡的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_DEALT_RESISTED)]
        public int DamageDealtResisted => _properties[DAMAGE_DEALT_RESISTED];

        /// <summary>
        /// 抵挡伤害量
        /// </summary>
        /// <remarks>
        /// 玩家抵挡的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_RESISTED)]
        public int DamageResisted => _properties[DAMAGE_RESISTED];

        /// <summary>
        /// 承受伤害量
        /// </summary>
        /// <remarks>
        /// 玩家承受的伤害总量，单位为1（♥）的1⁄10。
        /// </remarks>
        [CustomStatsKey(DAMAGE_TAKEN)]
        public int DamageTaken => _properties[DAMAGE_TAKEN];

        /// <summary>
        /// 死亡次数
        /// </summary>
        /// <remarks>
        /// 玩家死亡的次数。
        /// </remarks>
        [CustomStatsKey(DEATHS)]
        public int Deaths => _properties[DEATHS];

        /// <summary>
        /// 丢弃物品次数
        /// </summary>
        /// <remarks>
        /// 玩家丢弃物品的次数。
        /// </remarks>
        [CustomStatsKey(DROP)]
        public int Drop => _properties[DROP];

        /// <summary>
        /// 食用蛋糕片数
        /// </summary>
        /// <remarks>
        /// 玩家吃下的蛋糕片数。
        /// </remarks>
        [CustomStatsKey(EAT_CAKE_SLICE)]
        public int EatCakeSlice => _properties[EAT_CAKE_SLICE];

        /// <summary>
        /// 物品附魔次数
        /// </summary>
        /// <remarks>
        /// 玩家为物品使用附魔台附魔的次数。
        /// </remarks>
        [CustomStatsKey(ENCHANT_ITEM)]
        public int EnchantItem => _properties[ENCHANT_ITEM];

        /// <summary>
        /// 摔落高度
        /// </summary>
        /// <remarks>
        /// 玩家坠落的总距离，跳跃不计算在内。若玩家单次坠落距离超过1格，则会统计该次坠落的距离。
        /// </remarks>
        [CustomStatsKey(FALL_ONE_CM)]
        public int FallOneCm => _properties[FALL_ONE_CM];

        /// <summary>
        /// 炼药锅装水次数
        /// </summary>
        /// <remarks>
        /// 玩家使用水桶将炼药锅装满的次数。
        /// </remarks>
        [CustomStatsKey(FILL_CAULDRON)]
        public int FillCauldron => _properties[FILL_CAULDRON];

        /// <summary>
        /// 捕鱼数
        /// </summary>
        /// <remarks>
        /// 玩家捕到的鱼的总数。
        /// </remarks>
        [CustomStatsKey(FISH_CAUGHT)]
        public int FishCaught => _properties[FISH_CAUGHT];

        /// <summary>
        /// 飞行距离
        /// </summary>
        /// <remarks>
        /// 玩家同时向上、向前移动的总距离，玩家离地至少一格时移动的距离才会被统计。
        /// </remarks>
        [CustomStatsKey(FLY_ONE_CM)]
        public int FlyOneCm => _properties[FLY_ONE_CM];

        /// <summary>
        /// 骑乘快乐恶魂移动距离
        /// </summary>
        /// <remarks>
        /// 玩家骑乘快乐恶魂移动的总距离。
        /// </remarks>
        [CustomStatsKey(HAPPY_GHAST_ONE_CM)]
        public int HappyGhastOneCm => _properties[HAPPY_GHAST_ONE_CM];

        /// <summary>
        /// 骑马移动距离
        /// </summary>
        /// <remarks>
        /// 玩家骑马移动的总距离。
        /// </remarks>
        [CustomStatsKey(HORSE_ONE_CM)]
        public int HorseOneCm => _properties[HORSE_ONE_CM];

        /// <summary>
        /// 搜查发射器次数
        /// </summary>
        /// <remarks>
        /// 玩家和发射器交互的次数。
        /// </remarks>
        [CustomStatsKey(INSPECT_DISPENSER)]
        public int InspectDispenser => _properties[INSPECT_DISPENSER];

        /// <summary>
        /// 搜查投掷器次数
        /// </summary>
        /// <remarks>
        /// 玩家和投掷器交互的次数。
        /// </remarks>
        [CustomStatsKey(INSPECT_DROPPER)]
        public int InspectDropper => _properties[INSPECT_DROPPER];

        /// <summary>
        /// 搜查漏斗次数
        /// </summary>
        /// <remarks>
        /// 玩家和漏斗交互的次数。
        /// </remarks>
        [CustomStatsKey(INSPECT_HOPPER)]
        public int InspectHopper => _properties[INSPECT_HOPPER];

        /// <summary>
        /// 与铁砧交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和铁砧交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_ANVIL)]
        public int InteractWithAnvil => _properties[INTERACT_WITH_ANVIL];

        /// <summary>
        /// 与信标交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和信标交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_BEACON)]
        public int InteractWithBeacon => _properties[INTERACT_WITH_BEACON];

        /// <summary>
        /// 与高炉交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和高炉交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_BLAST_FURNACE)]
        public int InteractWithBlastFurnace => _properties[INTERACT_WITH_BLAST_FURNACE];

        /// <summary>
        /// 与酿造台交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和酿造台交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_BREWINGSTAND)]
        public int InteractWithBrewingstand => _properties[INTERACT_WITH_BREWINGSTAND];

        /// <summary>
        /// 与营火交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和营火交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_CAMPFIRE)]
        public int InteractWithCampfire => _properties[INTERACT_WITH_CAMPFIRE];

        /// <summary>
        /// 与制图台交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和制图台交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_CARTOGRAPHY_TABLE)]
        public int InteractWithCartographyTable => _properties[INTERACT_WITH_CARTOGRAPHY_TABLE];

        /// <summary>
        /// 与工作台交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和工作台交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_CRAFTING_TABLE)]
        public int InteractWithCraftingTable => _properties[INTERACT_WITH_CRAFTING_TABLE];

        /// <summary>
        /// 与熔炉交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和熔炉交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_FURNACE)]
        public int InteractWithFurnace => _properties[INTERACT_WITH_FURNACE];

        /// <summary>
        /// 与砂轮交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和砂轮交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_GRINDSTONE)]
        public int InteractWithGrindstone => _properties[INTERACT_WITH_GRINDSTONE];

        /// <summary>
        /// 与讲台交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和讲台交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_LECTERN)]
        public int InteractWithLectern => _properties[INTERACT_WITH_LECTERN];

        /// <summary>
        /// 与织布机交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和织布机交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_LOOM)]
        public int InteractWithLoom => _properties[INTERACT_WITH_LOOM];

        /// <summary>
        /// 与锻造台交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和锻造台交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_SMITHING_TABLE)]
        public int InteractWithSmithingTable => _properties[INTERACT_WITH_SMITHING_TABLE];

        /// <summary>
        /// 与烟熏炉交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和烟熏炉交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_SMOKER)]
        public int InteractWithSmoker => _properties[INTERACT_WITH_SMOKER];

        /// <summary>
        /// 与切石机交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和切石机交互的次数。
        /// </remarks>
        [CustomStatsKey(INTERACT_WITH_STONECUTTER)]
        public int InteractWithStonecutter => _properties[INTERACT_WITH_STONECUTTER];

        /// <summary>
        /// 跳跃次数
        /// </summary>
        /// <remarks>
        /// 玩家进行跳跃的次数。
        /// </remarks>
        [CustomStatsKey(JUMP)]
        public int Jump => _properties[JUMP];

        /// <summary>
        /// 游戏退出次数
        /// </summary>
        /// <remarks>
        /// 玩家点击“保存并退回到标题画面”按钮的次数。
        /// </remarks>
        [CustomStatsKey(LEAVE_GAME)]
        public int LeaveGame => _properties[LEAVE_GAME];

        /// <summary>
        /// 坐矿车移动距离
        /// </summary>
        /// <remarks>
        /// 玩家乘矿车移动的总距离。
        /// </remarks>
        [CustomStatsKey(MINECART_ONE_CM)]
        public int MinecartOneCm => _properties[MINECART_ONE_CM];

        /// <summary>
        /// 生物击杀数
        /// </summary>
        /// <remarks>
        /// 玩家击杀的生物总数。
        /// </remarks>
        [CustomStatsKey(MOB_KILLS)]
        public int MobKills => _properties[MOB_KILLS];

        /// <summary>
        /// 骑鹦鹉螺移动距离
        /// </summary>
        /// <remarks>
        /// 玩家骑鹦鹉螺移动的总距离。
        /// </remarks>
        [CustomStatsKey(NAUTILUS_ONE_CM)]
        public int NautilusOneCm => _properties[NAUTILUS_ONE_CM];

        /// <summary>
        /// 木桶打开次数
        /// </summary>
        /// <remarks>
        /// 玩家打开木桶的次数。
        /// </remarks>
        [CustomStatsKey(OPEN_BARREL)]
        public int OpenBarrel => _properties[OPEN_BARREL];

        /// <summary>
        /// 箱子打开次数
        /// </summary>
        /// <remarks>
        /// 玩家打开箱子的次数。
        /// </remarks>
        [CustomStatsKey(OPEN_CHEST)]
        public int OpenChest => _properties[OPEN_CHEST];

        /// <summary>
        /// 末影箱打开次数
        /// </summary>
        /// <remarks>
        /// 玩家打开末影箱的次数。
        /// </remarks>
        [CustomStatsKey(OPEN_ENDERCHEST)]
        public int OpenEnderchest => _properties[OPEN_ENDERCHEST];

        /// <summary>
        /// 潜影盒打开次数
        /// </summary>
        /// <remarks>
        /// 玩家打开潜影盒的次数。
        /// </remarks>
        [CustomStatsKey(OPEN_SHULKER_BOX)]
        public int OpenShulkerBox => _properties[OPEN_SHULKER_BOX];

        /// <summary>
        /// 骑猪移动距离
        /// </summary>
        /// <remarks>
        /// 玩家用鞍骑猪移动的总距离。
        /// </remarks>
        [CustomStatsKey(PIG_ONE_CM)]
        public int PigOneCm => _properties[PIG_ONE_CM];

        /// <summary>
        /// 音符盒播放次数
        /// </summary>
        /// <remarks>
        /// 玩家击打音符盒的次数。
        /// </remarks>
        [CustomStatsKey(PLAY_NOTEBLOCK)]
        public int PlayNoteblock => _properties[PLAY_NOTEBLOCK];

        /// <summary>
        /// 播放唱片数
        /// </summary>
        /// <remarks>
        /// 玩家用唱片机播放音乐唱片的次数。
        /// </remarks>
        [CustomStatsKey(PLAY_RECORD)]
        public int PlayRecord => _properties[PLAY_RECORD];

        /// <summary>
        /// 游戏时长
        /// </summary>
        /// <remarks>
        /// 玩家在游戏中经过的时长。该项统计的展示单位有秒、分钟、小时和天，游戏会根据实际情况选择最合理的单位展示数据。游戏暂停时，统计也会随之暂停。
        /// </remarks>
        [CustomStatsKey(PLAY_TIME)]
        public int PlayTime => _properties[PLAY_TIME];

        /// <summary>
        /// 玩家击杀数
        /// </summary>
        /// <remarks>
        /// 玩家（在开启PvP的服务器中）击杀的玩家总数，间接击杀的玩家不计算在内。
        /// </remarks>
        [CustomStatsKey(PLAYER_KILLS)]
        public int PlayerKills => _properties[PLAYER_KILLS];

        /// <summary>
        /// 盆栽种植数
        /// </summary>
        /// <remarks>
        /// 玩家将植物种进花盆的次数。
        /// </remarks>
        [CustomStatsKey(POT_FLOWER)]
        public int PotFlower => _properties[POT_FLOWER];

        /// <summary>
        /// 触发袭击次数
        /// </summary>
        /// <remarks>
        /// 玩家触发袭击的次数。
        /// </remarks>
        [CustomStatsKey(RAID_TRIGGER)]
        public int RaidTrigger => _properties[RAID_TRIGGER];

        /// <summary>
        /// 袭击胜利次数
        /// </summary>
        /// <remarks>
        /// 玩家战胜袭击的次数。
        /// </remarks>
        [CustomStatsKey(RAID_WIN)]
        public int RaidWin => _properties[RAID_WIN];

        /// <summary>
        /// 入眠次数
        /// </summary>
        /// <remarks>
        /// 玩家在床上睡觉的次数。
        /// </remarks>
        [CustomStatsKey(SLEEP_IN_BED)]
        public int SleepInBed => _properties[SLEEP_IN_BED];

        /// <summary>
        /// 潜行时间
        /// </summary>
        /// <remarks>
        /// 玩家潜行的时长。
        /// </remarks>
        [CustomStatsKey(SNEAK_TIME)]
        public int SneakTime => _properties[SNEAK_TIME];

        /// <summary>
        /// 疾跑距离
        /// </summary>
        /// <remarks>
        /// 玩家疾跑的总距离。
        /// </remarks>
        [CustomStatsKey(SPRINT_ONE_CM)]
        public int SprintOneCm => _properties[SPRINT_ONE_CM];

        /// <summary>
        /// 骑炽足兽移动距离
        /// </summary>
        /// <remarks>
        /// 玩家用鞍骑炽足兽移动的总距离。
        /// </remarks>
        [CustomStatsKey(STRIDER_ONE_CM)]
        public int StriderOneCm => _properties[STRIDER_ONE_CM];

        /// <summary>
        /// 游泳距离
        /// </summary>
        /// <remarks>
        /// 玩家游泳的总距离，在水下直立行走的距离不计算在内。
        /// </remarks>
        [CustomStatsKey(SWIM_ONE_CM)]
        public int SwimOneCm => _properties[SWIM_ONE_CM];

        /// <summary>
        /// 村民交互次数
        /// </summary>
        /// <remarks>
        /// 玩家和村民交互（打开其GUI）的次数。
        /// </remarks>
        [CustomStatsKey(TALKED_TO_VILLAGER)]
        public int TalkedToVillager => _properties[TALKED_TO_VILLAGER];

        /// <summary>
        /// 击中标靶次数
        /// </summary>
        /// <remarks>
        /// 玩家射中标靶的次数。
        /// </remarks>
        [CustomStatsKey(TARGET_HIT)]
        public int TargetHit => _properties[TARGET_HIT];

        /// <summary>
        /// 自上次死亡
        /// </summary>
        /// <remarks>
        /// 玩家自上次死亡以来经过的游戏时长。
        /// </remarks>
        [CustomStatsKey(TIME_SINCE_DEATH)]
        public int TimeSinceDeath => _properties[TIME_SINCE_DEATH];

        /// <summary>
        /// 自上次入眠
        /// </summary>
        /// <remarks>
        /// 玩家自上次在床上睡眠以来经过的游戏时长，用于生成幻翼。
        /// </remarks>
        [CustomStatsKey(TIME_SINCE_REST)]
        public int TimeSinceRest => _properties[TIME_SINCE_REST];

        /// <summary>
        /// 世界打开时间
        /// </summary>
        /// <remarks>
        /// 玩家打开世界的总时长。该项统计的展示单位有秒、分钟、小时和天，游戏会根据实际情况选择最合理的单位展示数据。与游戏时长不同的是，该项即使在游戏暂停时也会进行统计。
        /// </remarks>
        [CustomStatsKey(TOTAL_WORLD_TIME)]
        public int TotalWorldTime => _properties[TOTAL_WORLD_TIME];

        /// <summary>
        /// 村民交易次数
        /// </summary>
        /// <remarks>
        /// 玩家和村民进行交易的次数。
        /// </remarks>
        [CustomStatsKey(TRADED_WITH_VILLAGER)]
        public int TradedWithVillager => _properties[TRADED_WITH_VILLAGER];

        /// <summary>
        /// 陷阱箱触发次数
        /// </summary>
        /// <remarks>
        /// 玩家打开陷阱箱的次数。
        /// </remarks>
        [CustomStatsKey(TRIGGER_TRAPPED_CHEST)]
        public int TriggerTrappedChest => _properties[TRIGGER_TRAPPED_CHEST];

        /// <summary>
        /// 音符盒调音次数
        /// </summary>
        /// <remarks>
        /// 玩家和音符盒交互的次数。
        /// </remarks>
        [CustomStatsKey(TUNE_NOTEBLOCK)]
        public int TuneNoteblock => _properties[TUNE_NOTEBLOCK];

        /// <summary>
        /// 从炼药锅取水次数
        /// </summary>
        /// <remarks>
        /// 玩家从炼药锅中装取水、熔岩和细雪的次数。
        /// </remarks>
        [CustomStatsKey(USE_CAULDRON)]
        public int UseCauldron => _properties[USE_CAULDRON];

        /// <summary>
        /// 水面行走距离
        /// </summary>
        /// <remarks>
        /// 玩家在水面上下游动的总距离。
        /// </remarks>
        [CustomStatsKey(WALK_ON_WATER_ONE_CM)]
        public int WalkOnWaterOneCm => _properties[WALK_ON_WATER_ONE_CM];

        /// <summary>
        /// 行走距离
        /// </summary>
        /// <remarks>
        /// 玩家行走的总距离。
        /// </remarks>
        [CustomStatsKey(WALK_ONE_CM)]
        public int WalkOneCm => _properties[WALK_ONE_CM];

        /// <summary>
        /// 水下行走距离
        /// </summary>
        /// <remarks>
        /// 玩家在水下直立行走的总距离。
        /// </remarks>
        [CustomStatsKey(WALK_UNDER_WATER_ONE_CM)]
        public int WalkUnderWaterOneCm => _properties[WALK_UNDER_WATER_ONE_CM];

        public IEnumerable<string> Keys => _properties.Keys;

        public IEnumerable<int> Values => _properties.Values;

        public int Count => _properties.Count;

        public int this[string key] => _properties[key];

        public bool ContainsKey(string key) => _properties.ContainsKey(key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out int value) => _properties.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => _properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_properties).GetEnumerator();

        public static CustomStats Load(IReadOnlyDictionary<string, int> properties)
        {
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));

            Dictionary<string, int> result = [];
            foreach (string key in StatsKeys)
            {
                if (properties.TryGetValue(key, out var numberValue))
                    result.Add(key, numberValue);
                else
                    result.Add(key, 0);
            }

            return new CustomStats(result);
        }

        public static CustomStats Load(IReadOnlyDictionary<string, string> properties)
        {
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));

            Dictionary<string, int> result = [];
            foreach (string key in StatsKeys)
            {
                if (properties.TryGetValue(key, out var propertyValue) && int.TryParse(propertyValue, out var numberValue))
                    result.Add(key, numberValue);
                else
                    result.Add(key, 0);
            }

            return new CustomStats(result);
        }
    }
}
