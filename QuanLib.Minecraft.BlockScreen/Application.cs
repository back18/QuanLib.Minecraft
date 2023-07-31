using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public abstract class Application
    {
        protected Application(string arguments)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            FormManager = new();

            FormManager.AddedForm += FormManager_AddedForm;
            FormManager.RemovedForm += FormManager_RemovedForm;
        }

        public string Arguments { get; }

        public abstract IForm MainForm { get; }

        public FormManager FormManager { get; }

        public abstract object? Main();

        public virtual void Initialize()
        {
            FormManager.Forms.Add(MainForm);
        }

        private void Form_OnFormConse(IForm sender, EventArgs e)
        {
            FormManager.Forms.Remove(sender);
            ScreenContext? context = MCOS.GetMCOS().ScreenContextOf(sender);
            context?.RootForm.RemoveForm(sender);

            if (FormManager.Forms.Count == 0)
                Exit();
        }

        private void FormManager_AddedForm(FormManager sender, FormEventArgs e)
        {
            IControlInitializeHandling handling = e.Form;
            if (!handling.InitializeCompleted)
                handling.HandleAllInitialize();

            if (e.Form is IRootForm)
                return;

            MCOS os = MCOS.GetMCOS();
            IForm? initiator = os.ProcessOf(this)?.Initiator;
            if (initiator is IRootForm rootForm)
            {
                rootForm.AddForm(e.Form);
            }
            else
            {
                ScreenContext? context = null;
                if (initiator is not null)
                    context = os.ScreenContextOf(initiator);

                if (context is not null)
                {
                    context.RootForm.AddForm(e.Form);
                }
                else if (os.ScreenManager.ScreenContexts.Any())
                {
                    os.ScreenManager.ScreenContexts.FirstOrDefault().Value.RootForm.AddForm(e.Form);
                }
            }

            e.Form.FormClose += Form_OnFormConse;
        }

        private void FormManager_RemovedForm(FormManager sender, FormEventArgs e)
        {
            e.Form.FormClose -= Form_OnFormConse;
        }

        public virtual void Exit()
        {
            foreach (var form in FormManager.Forms.ToArray())
                form.CloseForm();
        }

        public static Application CreateApplication(Type appType, string arguments)
        {
            if (appType is null)
                throw new ArgumentNullException(nameof(appType));
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            if (!appType.IsSubclassOf(typeof(Application)))
                throw new ArgumentException("Type对象不是Application", nameof(appType));

            return appType.GetConstructor(new Type[] { typeof(string) })?.Invoke(new string[] { arguments }) as Application ??
                throw new ArgumentException("无法构建Application对象", nameof(appType));
        }

        public static Application CreateApplication<T>(string arguments) where T : Application
        {
            return CreateApplication(typeof(T), arguments);
        }
    }
}
