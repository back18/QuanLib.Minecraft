using MinecraftPlayerRanking;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class Switch : TextControl
    {
        public Switch()
        {
            _OnText = string.Empty;
            _OffText = string.Empty;

            Skin.BackgroundBlockID = Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Red);
            Skin.BackgroundBlockID_Selected = Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            ContentAnchor = ContentAnchor.Centered;
            RightClick += Switch_RightClick;
            OnSelected += Switch_OnSelected;
            OnDeselected += Switch_OnDeselected;
        }

        private void Switch_RightClick(Point position)
        {
            IsSelected = !IsSelected;
        }

        private void Switch_OnDeselected()
        {
            if (!string.IsNullOrEmpty(OffText))
                Text = OffText;
        }

        private void Switch_OnSelected()
        {
            if (!string.IsNullOrEmpty(OnText))
                Text = OnText;
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
    }
}
