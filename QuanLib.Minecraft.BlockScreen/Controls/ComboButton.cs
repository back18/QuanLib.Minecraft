using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class ComboButton<T> : ItemCollectionControl<T>, IButton where T : notnull
    {
        public ComboButton()
        {
            ReboundTime = 5;
            ReboundCountdown = 0;
            _Title = string.Empty;

            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            ContentLayout = ContentLayout.Centered;

            RightClick += ComboButton_RightClick;
            BeforeFrame += ComboButton_BeforeFrame;
            SelectedItemChanged += ComboButton_SelectedItemChanged;
        }

        public int ReboundTime { get; set; }

        public int ReboundCountdown { get; private set; }

        public string Title
        {
            get => _Title;
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RequestUpdateFrame();
                }
            }
        }
        private string _Title;

        private void ComboButton_SelectedItemChanged(T? oldItem, T? newItem)
        {
            string? item = ItemToString(newItem) ?? string.Empty;
            Text = string.IsNullOrEmpty(Title) ? item : $"{Title}: {item}";
        }

        private void ComboButton_RightClick(Point position)
        {
            if (!IsSelected)
            {
                ReboundCountdown = ReboundTime;
                IsSelected = true;
            }

            if (Items.Count < 1)
                return;

            if (SelectedItemIndex + 1 > Items.Count - 1)
                SelectedItemIndex = 0;
            else
                SelectedItemIndex++;
        }

        private void ComboButton_BeforeFrame()
        {
            if (IsSelected)
            {
                ReboundCountdown--;
                if (ReboundCountdown <= 0)
                    IsSelected = false;
            }
        }
    }
}
