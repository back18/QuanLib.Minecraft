using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public static class DialogBoxManager
    {
        public static MessageBoxButtons OpenMessageBox(IForm initiator, string title, string message, MessageBoxButtons buttons, Action<MessageBoxButtons>? callback = null)
        {
            if (initiator is null)
                throw new ArgumentNullException(nameof(initiator));
            if (title is null)
                throw new ArgumentNullException(nameof(title));
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            MCOS os = MCOS.Instance;
            Process? process = os.ProcessOf(initiator);
            FormContext? context = os.FormContextOf(initiator);
            if (process is null || process.ProcessState == ProcessState.Stopped ||
                context is null || context.FormState == FormState.Closed)
                return MessageBoxButtons.Cancel;

            MessageBoxForm form = new(initiator, title, message, buttons);
            process.Application.RunForm(form);

            if (process.ProcessState == ProcessState.Stopped || context.FormState == FormState.Closed)
                return MessageBoxButtons.Cancel;

            callback?.Invoke(form.DialogResult);
            return form.DialogResult;
        }

        public static async Task<MessageBoxButtons> OpenMessageBoxAsync(IForm initiator, string title, string message, MessageBoxButtons buttons, Action<MessageBoxButtons>? callback = null)
        {
           return await Task.Run(() => OpenMessageBox(initiator, title, message, buttons, callback));
        }
    }
}
