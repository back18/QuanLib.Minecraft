using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.Data;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
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
            ContentAnchor = AnchorPosition.Centered;

            Items.SelectedItemChanged += Items_SelectedItemChanged; ;
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

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

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

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            if (IsSelected)
            {
                ReboundCountdown--;
                if (ReboundCountdown <= 0)
                    IsSelected = false;
            }
        }

        private void Items_SelectedItemChanged(ItemCollection<T> sender, ObjectChangedEventArgs<T?> e)
        {
            SetText(e.NewItem);
        }

        private void SetText(T? item)
        {
            string text = Items.ItemToString(item);
            Text = string.IsNullOrEmpty(Title) ? text : $"{Title}: {text}";
        }
    }
}
