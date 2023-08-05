using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen
{
    public class DataScreenForm : WindowForm
    {
        public DataScreenForm()
        {
            DayTimeSyncTime = 16;
            GameTimeSyncTime = 1200;
            DayTimeSyncCountdown = 0;
            GameTimeSyncCountdown = 0;

            DayTime_Label = new();
            GameTime_Label = new();
        }

        private readonly Label DayTime_Label;

        private readonly Label GameTime_Label;

        public int DayTimeSyncTime { get; set; }

        public int GameTimeSyncTime { get; set; }

        public int DayTimeSyncCountdown { get; private set; }

        public int GameTimeSyncCountdown { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            Skin.SetAllBackgroundBlockID(BlockManager.Concrete.LightBlue);
            ClientPanel.Skin.SetAllBackgroundBlockID(BlockManager.Concrete.LightBlue);
            OnBeforeFrame(this, EventArgs.Empty);

            ClientPanel.SubControls.Add(DayTime_Label);
            DayTime_Label.ClientLocation = new(2, 2);
            DayTime_Label.Skin.SetAllBackgroundBlockID(Skin.BackgroundBlockID);

            ClientPanel.SubControls.Add(GameTime_Label);
            GameTime_Label.ClientLocation = new(2, 20);
            GameTime_Label.Skin.SetAllBackgroundBlockID(Skin.BackgroundBlockID);
        }

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            DayTimeSyncCountdown--;
            if (DayTimeSyncCountdown <= 0)
            {
                ServerCommandHelper command = MCOS.GetMCOS().MinecraftServer.CommandHelper;
                int gameDays = command.GetGameDays();
                TimeSpan dayTime = MinecraftUtil.DayTimeToTimeSpan(command.GetDayTime());
                DayTime_Label.Text = $"游戏时间：{gameDays}日{(int)dayTime.TotalHours}时{dayTime.Minutes}分";
                DayTimeSyncCountdown = DayTimeSyncTime;
            }

            GameTimeSyncCountdown--;
            if (GameTimeSyncCountdown <= 0)
            {
                TimeSpan gameTime = MinecraftUtil.GameTicksToTimeSpan(MCOS.GetMCOS().MinecraftServer.CommandHelper.GetGameTime());
                GameTime_Label.Text = $"开服时长：{gameTime.Days}天{gameTime.Hours}小时{gameTime.Minutes}分钟";
                GameTimeSyncCountdown = GameTimeSyncTime;
            }
        }
    }
}
