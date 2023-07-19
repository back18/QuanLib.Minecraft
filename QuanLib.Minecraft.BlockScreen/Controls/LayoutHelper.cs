using QuanLib.Minecraft.BlockScreen.BuiltInApps.FileExplorer;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public static class LayoutHelper
    {
        public static Point TopLayout(this Control control, Control? previous, Control next, int spacing, int start)
        {
            if (next is null)
                throw new ArgumentNullException(nameof(next));

            if (previous is null)
                return new(start, control.ClientSize.Height - next.Height - spacing);
            else
                return new(previous.LeftLocation, previous.TopLocation - next.Height - spacing);
        }

        public static Point TopLayout(this Control control, Control previous, Control next, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return TopLayout(control, previous, next, spacing, previous.LeftLocation);
        }

        public static Point BottomLayout(this Control control, Control? previous, int spacing, int start)
        {
            if (previous is null)
                return new(spacing, start);
            else
                return new(previous.LeftLocation, previous.BottomLocation + 1 + spacing);
        }

        public static Point BottomLayout(this Control control, Control previous, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return control.BottomLayout(previous, spacing, previous.LeftLocation);
        }

        public static Point LifeLayout(this Control control, Control? previous, Control next, int spacing, int start)
        {
            if (next is null)
                throw new ArgumentNullException(nameof(next));

            if (previous is null)
                return new(control.ClientSize.Width - next.Width - spacing, start);
            else
                return new(previous.LeftLocation - next.Width - spacing, previous.TopLocation);
        }

        public static Point LifeLayout(this Control control, Control previous, Control next, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return control.LifeLayout(previous, next, spacing, previous.TopLocation);
        }

        public static Point RightLayout(this Control control, Control? previous, int spacing, int start)
        {
            if (previous is null)
                return new(spacing, start);
            else
                return new(previous.RightLocation + 1 + spacing, previous.TopLocation);
        }

        public static Point RightLayout(this Control control, Control previous, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return control.RightLayout(previous, spacing, previous.TopLocation);
        }

        public static Point VerticalCenterLayout(this Control control, Control subControl, int start)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(start, control.Height / 2 - subControl.Height / 2);
        }

        public static Point HorizontalCenterLayout(this Control control, Control subControl, int start)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(control.Width / 2 - subControl.Width / 2, start);
        }

        public static Point CenterLayout(this Control control, Control subControl)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(control.Width / 2 - subControl.Width / 2, control.Height / 2 - subControl.Height / 2);
        }

        public static T[] FillLayout<T>(this Control control, int spacing, IReadOnlyList<T> controls, int startIndex = 0) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            List<T> result = new();
            T? previous = null;
            for (int i = startIndex; i < controls.Count; i++)
            {
                if (previous is null || previous.RightLocation + spacing + controls[i].Width <= control.Width - control.BorderWidth)
                    controls[i].ClientLocation = control.RightLayout(previous, spacing, spacing);
                else if (previous.BottomLocation + 1 + spacing + controls[i].Height <= control.Height - control.BorderWidth)
                    controls[i].ClientLocation = control.BottomLayout(null, spacing, previous.BottomLocation + 1 + spacing);
                else
                    break;
                result.Add(controls[i]);
                previous = controls[i];
            }
            return result.ToArray();
        }

        public static T[][] FillLayouPagingt<T>(this Control control, int spacing, IReadOnlyList<T> controls) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            List<T[]> result = new();
            int count = 0;
            while (count < controls.Count)
            {
                T[] page = control.FillLayout(spacing, controls, count);
                result.Add(page);
                count += page.Length;
            }
            return result.ToArray();
        }
    }
}
