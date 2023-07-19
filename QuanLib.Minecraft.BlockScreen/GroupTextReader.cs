using QuanLib.Minecraft.BlockScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class GroupTextReader : IMCOSComponent, ISwitchable
    {
        public GroupTextReader()
        {
            OperatorList = new();
            _textCaches = new();
            InitialText = string.Empty;

            OnTextUpdate += (arg1, arg2) => { };
        }

        private readonly Dictionary<string, string> _textCaches;

        private bool _runing;

        public bool Runing => _runing;

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

        /// <summary>
        /// 初始文本
        /// </summary>
        public string InitialText { get; set; }

        /// <summary>
        /// 文本读取器的操作员列表
        /// </summary>
        public List<string> OperatorList { get; }

        /// <summary>
        /// 当文本更新时
        /// </summary>
        public event Action<string, string> OnTextUpdate;

        public void Start()
        {
            ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;

            _runing = true;
            while (_runing)
            {
                foreach (var op in OperatorList)
                {
                    if (OperatorList.Count > 0 &&
                        command.TryGetPlayerSelectedItemSlot(op, out var solt) &&
                        command.TryGetPlayerItem(op, solt, out var item) &&
                        item.ID == "minecraft:writable_book")
                    {
                        if (!_textCaches.TryGetValue(op, out var textCache))
                        {
                            if (!string.IsNullOrEmpty(InitialText))
                            {
                                command.SetPlayerHotbarItemAsync(op, solt, $"minecraft:writable_book{{pages:[\"{InitialText}\"]}}").Wait();
                                _textCaches[op] = InitialText;
                            }
                        }
                        else if (item.Tag.TryGetValue("pages", out var pagesTag) && pagesTag is string[] texts && texts.Length > 0)
                        {
                            if (texts[0] != textCache)
                            {
                                OnTextUpdate.Invoke(op, texts[0]);
                                _textCaches[op] = texts[0];
                            }
                        }
                        else if (!string.IsNullOrEmpty(textCache))
                        {
                            OnTextUpdate.Invoke(op, string.Empty);
                            _textCaches[op] = string.Empty;
                        }
                    }
                }
                Thread.Sleep(40);
            }
        }

        public void Handle()
        {
            Screen screen = MCOS.Screen;
            ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;


        }

        public void Stop()
        {
            _runing = false;
            _textCaches.Clear();
        }
    }
}
