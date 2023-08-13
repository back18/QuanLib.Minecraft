using PostSharp.Extensibility;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public abstract class DialogBoxForm<Result> : WindowForm
    {
        protected DialogBoxForm(Form initiator, string title)
        {
            _initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
            _title = title ?? throw new ArgumentNullException(nameof(title));

            AllowDeselected = false;
            AllowResize = false;
            TitleBar.ButtonsToShow = FormButtons.Close;
        }

        protected readonly Form _initiator;

        protected readonly string _title;

        public abstract Result DialogResult { get; protected set; }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            Text = _title;
        }
    }
}
