using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public abstract class Application : IMCOSComponent
    {
        protected Application()
        {
            _forms = new();
        }

        public readonly List<Form> _forms;

        public MCOS MCOS
        {
            get
            {
                if (_MCOS is null)
                    throw new InvalidOperationException();
                return _MCOS;
            }
            internal set => _MCOS = value;
        }
        private MCOS? _MCOS;

        public Process Process
        {
            get
            {
                if (_Process is null)
                    throw new InvalidOperationException();
                return _Process;
            }
            internal set => _Process = value;
        }
        private Process? _Process;

        public abstract string AppID { get; }

        public abstract string AppName { get; }

        public IReadOnlyList<Form> Forms => _forms;

        public abstract Form MainForm { get; }

        public Form ForegroundForm
        {
            get
            {
                _ForegroundForm ??= MainForm;
                return _ForegroundForm;
            }
            private set => _ForegroundForm = value;
        }
        private Form? _ForegroundForm;

        public virtual void Initialize()
        {
            RegisterForm(MainForm);
        }

        public abstract object? Main(string[] args);

        public abstract void Exit();

        public void RegisterForm(Form form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            form.Application = this;
            _forms.Add(form);
            form.HandleInitialize();
            form.HandleOnInitComplete1();
            form.HandleOnInitComplete2();
            form.HandleOnInitComplete3();
        }

        public static Application CreateApplication(Type appType)
        {
            if (appType is null)
                throw new ArgumentNullException(nameof(appType));
            if (!appType.IsSubclassOf(typeof(Application)))
                throw new ArgumentException("Type对象不是Application", nameof(appType));

            return (Application)(Activator.CreateInstance(appType) ?? throw new ArgumentException("无法构建Application对象", nameof(appType)));
        }

        public static Application CreateApplication<T>() where T : Application
        {
            return CreateApplication(typeof(T));
        }
    }
}
