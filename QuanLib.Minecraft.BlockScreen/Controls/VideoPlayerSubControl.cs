using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class VideoPlayerSubControl : Control
    {
        protected VideoPlayerSubControl(VideoPlayer owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        protected readonly VideoPlayer _owner;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();
        }
    }
}
