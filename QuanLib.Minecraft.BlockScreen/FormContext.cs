using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    /// <summary>
    /// 窗体运行时上下文
    /// </summary>
    public class FormContext
    {
        public FormContext(Application application, IForm form)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            Form = form ?? throw new ArgumentNullException(nameof(form));

            if (form is IRootForm rootForm1)
            {
                RootForm = rootForm1;
            }
            else
            {
                MCOS os = MCOS.GetMCOS();
                IForm? initiator = os.ProcessOf(Application)?.Initiator;
                if (initiator is IRootForm rootForm2)
                {
                    RootForm = rootForm2;
                }
                else
                {
                    ScreenContext? context = null;
                    if (initiator is not null)
                        context = os.ScreenContextOf(initiator);

                    if (context is not null)
                        RootForm = context.RootForm;
                    else if (os.ScreenManager.ScreenList.Any())
                        RootForm = os.ScreenManager.ScreenList.FirstOrDefault().Value.RootForm;
                    else
                        throw new InvalidOperationException();
                }
            }

            IsMinimize = false;
            Runing = false;
            _close = new(false);
        }

        private readonly AutoResetEvent _close;

        public bool IsMinimize { get; private set; }

        public bool Runing { get; private set; }

        public IRootForm RootForm { get; private set; }

        public Application Application { get; }

        public IForm Form { get; }

        public void MinimizeForm()
        {
            if (!IsMinimize)
            {
                if (Form is not IRootForm && RootForm.ContainsForm(Form))
                    RootForm.RemoveForm(Form);
                IsMinimize = true;
                Form.HandleFormMinimize(EventArgs.Empty);
            }
        }

        public void UnminimizeForm()
        {
            if (IsMinimize)
            {
                if (Form is not IRootForm && !RootForm.ContainsForm(Form))
                    RootForm.AddForm(Form);
                IsMinimize = false;
                Form.HandleFormUnminimize(EventArgs.Empty);
            }
        }

        public void LoadForm()
        {
            if (!Runing)
            {
                if (Form is not IRootForm && !RootForm.ContainsForm(Form))
                    RootForm.AddForm(Form);
                Runing = true;
                Form.HandleFormLoad(EventArgs.Empty);
            }
        }

        public void CloseForm()
        {
            if (Runing)
            {
                if (Form is not IRootForm && RootForm.ContainsForm(Form))
                    RootForm.RemoveForm(Form);
                Runing = false;
                Form.HandleFormClose(EventArgs.Empty);
                _close.Set();
            }
        }

        public void WaitForFormClose()
        {
            _close.WaitOne();
        }
    }
}
