using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public static class DialogBoxHelper
    {
        public static R OpenDialogBox<R>(IForm initiator, DialogBoxForm<R> dialogBox, Action<R>? callback = null)
        {
            if (dialogBox is null)
                throw new ArgumentNullException(nameof(dialogBox));
            if (initiator is null)
                throw new ArgumentNullException(nameof(initiator));

            MCOS os = MCOS.Instance;
            Process? process = os.ProcessOf(initiator);
            FormContext? context = os.FormContextOf(initiator);
            if (process is null || process.ProcessState == ProcessState.Stopped ||
                context is null || context.FormState == FormState.Closed)
                return dialogBox.DefaultResult;

            process.Application.RunForm(dialogBox);

            if (process.ProcessState == ProcessState.Stopped || context.FormState == FormState.Closed)
                return dialogBox.DefaultResult;

            callback?.Invoke(dialogBox.DialogResult);
            return dialogBox.DialogResult;
        }

        public static async Task<R> OpenDialogBoxAsync<R>(IForm initiator, DialogBoxForm<R> dialogBox, Action<R>? callback = null)
        {
            return await Task.Run(() => OpenDialogBox(initiator, dialogBox, callback));
        }

        public static MessageBoxButtons OpenMessageBox(IForm initiator, string title, string message, MessageBoxButtons buttons, Action<MessageBoxButtons>? callback = null)
        {
            MessageBoxForm dialogBox = new(initiator, title, message, buttons);
            return OpenDialogBox(initiator, dialogBox, callback);
        }

        public static async Task<MessageBoxButtons> OpenMessageBoxAsync(IForm initiator, string title, string message, MessageBoxButtons buttons, Action<MessageBoxButtons>? callback = null)
        {
            MessageBoxForm dialogBox = new(initiator, title, message, buttons);
            return await OpenDialogBoxAsync(initiator, dialogBox, callback);
        }

        public static string OpenTextInputBox(IForm initiator, string title, Action<string>? callback = null)
        {
            TextInputBoxForm dialogBox = new(initiator, title);
            return OpenDialogBox(initiator, dialogBox, callback);
        }

        public static async Task<string> OpenTextInputBoxAsync(IForm initiator, string title, Action<string>? callback = null)
        {
            TextInputBoxForm dialogBox = new(initiator, title);
            return await OpenDialogBoxAsync(initiator, dialogBox, callback);
        }

        public static ApplicationInfo? OpenApplicationListBox(IForm initiator, string title, Action<ApplicationInfo?>? callback = null)
        {
            ApplicationListBoxForm dialogBox = new(initiator, title);
            return OpenDialogBox(initiator, dialogBox, callback);
        }

        public static async Task<ApplicationInfo?> OpenApplicationListBoxAsync(IForm initiator, string title, Action<ApplicationInfo?>? callback = null)
        {
            ApplicationListBoxForm dialogBox = new(initiator, title);
            return await OpenDialogBoxAsync(initiator, dialogBox, callback);
        }
    }
}
