using QuanLib.Minecraft.Datas;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class ComboButton<T> : TextControl, IItemContainer<T>, IButton where T : notnull
    {
        public ComboButton()
        {
            Items = new();
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
            Items.SelectedItemChanged += Items_SelectedItemChanged;
        }

        public ItemCollection<T> Items { get; }

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

        public override void Initialize()
        {
            base.Initialize();

            SetText(Items.SelectedItem);
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

            if (Items.SelectedItemIndex + 1 > Items.Count - 1)
                Items.SelectedItemIndex = 0;
            else
                Items.SelectedItemIndex++;
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

        private void Items_SelectedItemChanged(T? oldItem, T? newItem)
        {
            SetText(newItem);
        }

        private void SetText(T? item)
        {
            string text = Items.ItemToString(item);
            Text = string.IsNullOrEmpty(Title) ? text : $"{Title}: {text}";
        }
    }
}
