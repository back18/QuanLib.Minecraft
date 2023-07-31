using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class Switch : TextControl
    {
        public Switch()
        {
            _OnText = string.Empty;
            _OffText = string.Empty;

            Skin.BackgroundBlockID = Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Red);
            Skin.BackgroundBlockID_Selected = Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            ContentAnchor = AnchorPosition.Centered;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (IsSelected)
            {
                if (!string.IsNullOrEmpty(OnText))
                    Text = OnText;
            }
            else
            {
                if (!string.IsNullOrEmpty(OffText))
                    Text = OffText;
            }
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            IsSelected = !IsSelected;
        }

        protected override void OnControlSelected(Control sender, EventArgs e)
        {
            base.OnControlSelected(sender, e);

            if (!string.IsNullOrEmpty(OnText))
                Text = OnText;
        }

        protected override void OnControlDeselected(Control sender, EventArgs e)
        {
            base.OnControlDeselected(sender, e);

            if (!string.IsNullOrEmpty(OffText))
                Text = OffText;
        }

        public string OnText
        {
            get => _OnText;
            set
            {
                _OnText = value;
                if (IsSelected)
                    RequestUpdateFrame();
            }
        }
        private string _OnText;

        public string OffText
        {
            get => _OffText;
            set
            {
                _OffText = value;
                if (!IsSelected)
                    RequestUpdateFrame();
            }
        }
        private string _OffText;
    }
}
