using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Settings
{
    public class SettingsForm : WindowForm
    {
        public SettingsForm()
        {
            GetInteraction_Button = new();
            ClearInteraction_Button = new();
        }

        private readonly Button GetInteraction_Button;

        private readonly Button ClearInteraction_Button;

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(GetInteraction_Button);
            GetInteraction_Button.Text = "获取交互";
            GetInteraction_Button.ClientSize = new(64, 16);
            GetInteraction_Button.ClientLocation = new(2, 2);
            GetInteraction_Button.RightClick += GetInteraction_Button_RightClick;

            ClientPanel.SubControls.Add(ClearInteraction_Button);
            ClearInteraction_Button.Text = "清除交互";
            ClearInteraction_Button.ClientSize = new(64, 16);
            ClearInteraction_Button.ClientLocation = ClientPanel.BottomLayout(GetInteraction_Button, 2);
            ClearInteraction_Button.RightClick += ClearInteraction_Button_RightClick;
        }

        private void GetInteraction_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            string? player = GetScreenContext()?.Screen.InputHandler.CurrentPlayer;
            if (player is null)
                return;

            MCOS.Instance.InteractionManager.Items.TryAdd(player, out _);
        }

        private void ClearInteraction_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            string? player = GetScreenContext()?.Screen.InputHandler.CurrentPlayer;
            if (player is null)
                return;

            if (MCOS.Instance.InteractionManager.Items.TryGetValue(player, out var interaction))
                interaction.Dispose();
        }
    }
}
