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
                MCOS os = MCOS.Instance;
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

            FormState = FormState.NotLoaded;
            IsMinimize = false;
            Runing = false;
            _close = new(false);
        }

        private readonly AutoResetEvent _close;

        public FormState FormState { get; private set; }

        public bool IsMinimize { get; private set; }

        public bool Runing { get; private set; }

        public IRootForm RootForm { get; private set; }

        public Application Application { get; }

        public IForm Form { get; }

        public void Handle()
        {
            if (Form is IRootForm)
                return;

            switch (FormState)
            {
                case FormState.NotLoaded:
                    break;
                case FormState.Loading:
                    if (!RootForm.ContainsForm(Form))
                    {
                        RootForm.AddForm(Form);
                        Form.HandleFormLoad(EventArgs.Empty);
                    }
                    FormState = FormState.Active;
                    break;
                case FormState.Active:
                    if (!RootForm.ContainsForm(Form))
                    {
                        RootForm.AddForm(Form);
                        Form.HandleFormUnminimize(EventArgs.Empty);
                    }
                    break;
                case FormState.Minimize:
                    if (RootForm.ContainsForm(Form))
                    {
                        RootForm.RemoveForm(Form);
                        Form.HandleFormMinimize(EventArgs.Empty);
                    }
                    break;
                case FormState.Closed:
                    if (RootForm.ContainsForm(Form))
                    {
                        RootForm.RemoveForm(Form);
                        Form.HandleFormClose(EventArgs.Empty);
                        _close.Set();
                    }
                    break;
                default:
                    break;
            }
        }

        public void LoadForm()
        {
            if (!Runing)
            {
                if (Form is not IRootForm && !RootForm.ContainsForm(Form))
                {
                    Runing = true;
                    FormState = FormState.Loading;
                }
            }
        }

        public void CloseForm()
        {
            if (Runing)
            {
                if (Form is not IRootForm && RootForm.ContainsForm(Form))
                {
                    Runing = false;
                    FormState = FormState.Closed;
                }
            }
        }

        public void MinimizeForm()
        {
            if (!IsMinimize)
            {
                if (Form is not IRootForm && RootForm.ContainsForm(Form) && Form.AllowDeselected)
                {
                    IsMinimize = true;
                    FormState = FormState.Minimize;
                }
            }
        }

        public void UnminimizeForm()
        {
            if (IsMinimize)
            {
                if (Form is not IRootForm && !RootForm.ContainsForm(Form))
                {
                    IsMinimize = false;
                    FormState = FormState.Active;
                }
            }
        }

        public void WaitForFormClose()
        {
            _close.WaitOne();
        }
    }
}
