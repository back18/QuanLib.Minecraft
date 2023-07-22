using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class TextControl : Control
    {
        protected TextControl()
        {
            OnTextUpdateNow += TextControl_OnTextUpdateNow;
        }

        public override Frame RenderingFrame()
        {
            Frame? image = Skin.GetBackgroundImage()?.GetFrame();

            switch (ControlContent)
            {
                case ControlContent.None:
                    return base.RenderingFrame();
                case ControlContent.Text:
                    FrameBuilder fb = RenderingText(Skin.GetForegroundBlockID(), Skin.GetBackgroundBlockID());
                    CorrectSize(fb);
                    return fb.ToFrame();
                case ControlContent.Image:
                    CorrectSize(image!);
                    return image!;
                case ControlContent.Text | ControlContent.Image:
                    FrameBuilder text = RenderingText(Skin.GetForegroundBlockID(), string.Empty);
                    image!.Overwrite(text.ToFrame(), new(0, 0));
                    CorrectSize(image!);
                    return image!;
                default:
                    throw new InvalidOperationException();
            }

            FrameBuilder RenderingText(string foreground, string background)
            {
                FrameBuilder result = new();
                switch (ContentAnchor)
                {
                    case ContentAnchor.UpperLeft:
                    case ContentAnchor.LowerLeft:
                    case ContentAnchor.Centered:
                        foreach (var c in Text)
                        {
                            result.AddRight(Frame.BuildFrame(MCOS.DefaultFont[c].GetBitMap(), foreground, background));
                            if (result.Width >= Width)
                                break;
                        }
                        break;
                    case ContentAnchor.UpperRight:
                    case ContentAnchor.LowerRight:
                        for (int i = Text.Length - 1; i >= 0; i--)
                        {
                            result.AddLeft(Frame.BuildFrame(MCOS.DefaultFont[Text[i]].GetBitMap(), foreground, background));
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
