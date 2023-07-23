using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI.Controls
{
    public abstract class TextControl : Control
    {
        protected TextControl()
        {
            OnTextUpdateNow += TextControl_OnTextUpdateNow;
        }

        public override AbstractFrame RenderingFrame()
        {
            ArrayFrame? image = Skin.GetBackgroundImage()?.GetFrame();

            switch (ControlContent)
            {
                case ControlContent.None:
                    return base.RenderingFrame();
                case ControlContent.Text:
                    LinkedFrame fb = RenderingText(Skin.GetForegroundBlockID(), Skin.GetBackgroundBlockID());
                    return fb;
                case ControlContent.Image:
                    return image!;
                case ControlContent.Text | ControlContent.Image:
                    LinkedFrame text = RenderingText(Skin.GetForegroundBlockID(), string.Empty);
                    image!.Overwrite(text.ToArrayFrame(), new(0, 0));
                    return image!;
                default:
                    throw new InvalidOperationException();
            }

            LinkedFrame RenderingText(string foreground, string background)
            {
                LinkedFrame result = new();
                switch (ContentAnchor)
                {
                    case ContentAnchor.UpperLeft:
                    case ContentAnchor.LowerLeft:
                    case ContentAnchor.Centered:
                        foreach (var c in Text)
                        {
                            result.AddRight(ArrayFrame.BuildFrame(MCOS.DefaultFont[c].GetBitMap(), foreground, background));
                            if (result.Width >= Width)
                                break;
                        }
                        break;
                    case ContentAnchor.UpperRight:
                    case ContentAnchor.LowerRight:
                        for (int i = Text.Length - 1; i >= 0; i--)
                        {
                            result.AddLeft(ArrayFrame.BuildFrame(MCOS.DefaultFont[Text[i]].GetBitMap(), foreground, background));
                            if (result.Width >= Width)
                                break;
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                return result;
            }
        }

        public override void AutoSetSize()
        {
            ClientSize = MCOS.DefaultFont.GetTotalSize(Text);
        }

        private void TextControl_OnTextUpdateNow(string oldText, string newText)
        {
            if (AutoSize)
                AutoSetSize();
        }
    }
}
