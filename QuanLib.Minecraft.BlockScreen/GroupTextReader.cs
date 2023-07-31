using QuanLib.Minecraft.BlockScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class GroupTextReader : ISwitchable
    {
        public GroupTextReader()
        {
            OperatorList = new();
            _textCaches = new();
            InitialText = string.Empty;

            OnTextChanged += (arg1, arg2) => { };
        }

        private readonly Dictionary<string, string> _textCaches;

        private bool _runing;

        public bool Runing => _runing;

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
        public event Action<string, string> OnTextChanged;

        public void Start()
        {
            ServerCommandHelper command = MCOS.GetMCOS().MinecraftServer.CommandHelper;

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
                                command.SetPlayerHotbarItem(op, solt, $"minecraft:writable_book{{pages:[\"{InitialText}\"]}}");
                                _textCaches[op] = InitialText;
                            }
                        }
                        else if (item.Tag.TryGetValue("pages", out var pagesTag) && pagesTag is string[] texts && texts.Length > 0)
                        {
                            if (texts[0] != textCache)
                            {
                                OnTextChanged.Invoke(op, texts[0]);
                                _textCaches[op] = texts[0];
                            }
                        }
                        else if (!string.IsNullOrEmpty(textCache))
                        {
                            OnTextChanged.Invoke(op, string.Empty);
                            _textCaches[op] = string.Empty;
                        }
                    }
                }
                Thread.Sleep(40);
            }
        }

        public void Handle()
        {

        }

        public void Stop()
        {
            _runing = false;
            _textCaches.Clear();
        }
    }
}
