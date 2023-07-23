using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections;
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
            ActiveForms = new(this);

            OnAddedForm += Application_OnAddedForm;
            OnRemovedForm += Application_OnRemovedForm;
        }

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

        public abstract Form MainForm { get; }

        public FormCollection ActiveForms { get; }

        public event Action<IForm> OnAddedForm;

        public event Action<IForm> OnRemovedForm;

        private void Application_OnAddedForm(IForm form)
        {
            form.SetApplication(this);
            IControlInitializeHandling handling = form;
            if (!handling.InitializeCompleted)
                handling.HandleAllInitialize();

            form.OnFormClose += Form_OnFormConse;
        }

        private void Application_OnRemovedForm(IForm form)
        {
            form.OnFormClose -= Form_OnFormConse;
        }

        private void Form_OnFormConse(IForm form)
        {
            ActiveForms.Remove(form);

            if (ActiveForms.Count == 0)
                Exit();
        }

        public abstract object? Main(string[] args);

        public virtual void Initialize()
        {
            ActiveForms.Add(MainForm);
        }

        public virtual void Exit()
        {
            foreach (var form in ActiveForms)
                form.CloseForm();
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

        public class FormCollection : IList<IForm>, IReadOnlyList<IForm>
        {
            public FormCollection(Application owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _forms = new();
            }

            private readonly Application _owner;

            private readonly List<IForm> _forms;

            public IForm this[int index] => _forms[index];

            IForm IList<IForm>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public int Count => _forms.Count;

            public bool IsReadOnly => false;

            public void Add(IForm item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                _forms.Add(item);
                _owner.OnAddedForm.Invoke(item);
            }

            public bool TryAdd(IForm item)
            {
                if (_forms.Contains(item))
                    return false;

                Add(item);
                return true;
            }

            public bool Remove(IForm item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                if (!Remove(item))
                    return false;

                _owner.OnRemovedForm.Invoke(item);
                return true;
            }

            public void RemoveAt(int index)
            {
                _forms.RemoveAt(index);
            }

            public void Clear()
            {
                _forms.Clear();
            }

            public bool Contains(IForm item)
            {
                return _forms.Contains(item);
            }

            public int IndexOf(IForm item)
            {
                return _forms.IndexOf(item);
            }

            public void CopyTo(IForm[] array, int arrayIndex)
            {
                _forms.CopyTo(array, arrayIndex);
            }

            public IEnumerator<IForm> GetEnumerator()
            {
                return _forms.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_forms).GetEnumerator();
            }

            void IList<IForm>.Insert(int index, IForm item)
            {
                throw new NotSupportedException();
            }
        }
    }
}
