using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public abstract class DialogBoxForm<R> : WindowForm
    {
        protected DialogBoxForm(IForm initiator, string title)
        {
            _initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
            _title = title ?? throw new ArgumentNullException(nameof(title));

            AllowDeselected = false;
            AllowResize = false;
            TitleBar.ButtonsToShow = FormButtons.Close;
        }

        protected readonly IForm _initiator;

        protected readonly string _title;
        
        public abstract R DefaultResult { get; }

        public abstract R DialogResult { get; protected set; }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            Text = _title;
        }

        public void CenterOnInitiatorForm()
        {
            ClientLocation = new(
                _initiator.ClientLocation.X + (_initiator.ClientSize.Width + _initiator.BorderWidth - Width) / 2,
                _initiator.ClientLocation.Y + (_initiator.ClientSize.Height + _initiator.BorderWidth - Height) / 2);
        }
    }
}
