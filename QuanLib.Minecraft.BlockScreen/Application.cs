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
        public abstract object? Main(string[] args);

        public object? RunForm(IForm form)
        {
            FormContext context = new(this, form);
            MCOS.Instance.FormManager.FormList.Add(context);
            context.LoadForm();
            context.WaitForFormClose();
            return form.ReturnValue;
        }

        public FormContext[] GetForms()
        {
            List<FormContext> result = new();
            foreach (var context in MCOS.Instance.FormManager.FormList)
                if (context.Application == this)
                    result.Add(context);

            return result.ToArray();
        }

        public static Application CreateApplication(Type appType)
        {
            if (appType is null)
                throw new ArgumentNullException(nameof(appType));
            if (!appType.IsSubclassOf(typeof(Application)))
                throw new ArgumentException("Type对象不是Application", nameof(appType));

            return Activator.CreateInstance(appType) as Application ?? throw new ArgumentException("无法构建Application对象", nameof(appType));
        }

        public static Application CreateApplication<T>() where T : Application
        {
            return CreateApplication(typeof(T));
        }
    }
}
