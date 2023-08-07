using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.Utility
{
    public static class LayoutHelper
    {
        public static Point TopLayout(this Control source, Control? previous, Control next, int spacing, int start)
        {
            if (next is null)
                throw new ArgumentNullException(nameof(next));

            if (previous is null)
                return new(start, source.ClientSize.Height - next.Height - spacing);
            else
                return new(previous.LeftLocation, previous.TopLocation - next.Height - spacing);
        }

        public static Point TopLayout(this Control source, Control previous, Control next, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return TopLayout(source, previous, next, spacing, previous.LeftLocation);
        }

        public static Point BottomLayout(this Control source, Control? previous, int spacing, int start)
        {
            if (previous is null)
                return new(spacing, start);
            else
                return new(previous.LeftLocation, previous.BottomLocation + 1 + spacing);
        }

        public static Point BottomLayout(this Control source, Control previous, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return source.BottomLayout(previous, spacing, previous.LeftLocation);
        }

        public static Point LeftLayout(this Control source, Control? previous, Control next, int spacing, int start)
        {
            if (next is null)
                throw new ArgumentNullException(nameof(next));

            if (previous is null)
                return new(source.ClientSize.Width - next.Width - spacing, start);
            else
                return new(previous.LeftLocation - next.Width - spacing, previous.TopLocation);
        }

        public static Point LeftLayout(this Control source, Control previous, Control next, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return source.LeftLayout(previous, next, spacing, previous.TopLocation);
        }

        public static Point RightLayout(this Control source, Control? previous, int spacing, int start)
        {
            if (previous is null)
                return new(spacing, start);
            else
                return new(previous.RightLocation + 1 + spacing, previous.TopLocation);
        }

        public static Point RightLayout(this Control source, Control previous, int spacing)
        {
            if (previous is null)
                throw new ArgumentNullException(nameof(previous));

            return source.RightLayout(previous, spacing, previous.TopLocation);
        }

        public static Point VerticalCenterLayout(this Control source, Control subControl, int start)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(start, source.ClientSize.Height / 2 - subControl.Height / 2);
        }

        public static Point HorizontalCenterLayout(this Control source, Control subControl, int start)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(source.ClientSize.Width / 2 - subControl.Width / 2, start);
        }

        public static Point CenterLayout(this Control source, Control subControl)
        {
            if (subControl is null)
                throw new ArgumentNullException(nameof(subControl));

            return new(source.Width / 2 - subControl.Width / 2, source.Height / 2 - subControl.Height / 2);
        }

        public static void ForceFillRightLayout<T>(this Control source, int spacing, IEnumerable<T> controls) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            T? previous = null;
            foreach (var control in controls)
            {
                if (previous is null || previous.BottomLocation + 1 + spacing + control.Height <= source.Height - source.BorderWidth)
                    control.ClientLocation = source.BottomLayout(previous, spacing, spacing);
                else
                    control.ClientLocation = source.RightLayout(null, previous.RightLocation + 1 + spacing, spacing);

                previous = control;
            }
        }

        public static void ForceFillDownLayout<T>(this Control source, int spacing, IEnumerable<T> controls) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            T? previous = null;
            foreach (var control in controls)
            {
                if (previous is null || previous.RightLocation + spacing + control.Width <= source.Width - source.BorderWidth)
                    control.ClientLocation = source.RightLayout(previous, spacing, spacing);
                else
                    control.ClientLocation = source.BottomLayout(null, spacing, previous.BottomLocation + 1 + spacing);

                previous = control;
            }
        }

        public static T[] FillDownLayout<T>(this Control source, int spacing, IReadOnlyList<T> controls, int startIndex = 0) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            List<T> result = new();
            T? previous = null;
            for (int i = startIndex; i < controls.Count; i++)
            {
                if (previous is null || previous.RightLocation + spacing + controls[i].Width <= source.Width - source.BorderWidth)
                    controls[i].ClientLocation = source.RightLayout(previous, spacing, spacing);
                else if (previous.BottomLocation + 1 + spacing + controls[i].Height <= source.Height - source.BorderWidth)
                    controls[i].ClientLocation = source.BottomLayout(null, spacing, previous.BottomLocation + 1 + spacing);
                else
                    break;
                result.Add(controls[i]);
                previous = controls[i];
            }
            return result.ToArray();
        }

        public static T[][] FillLayouDownPagingt<T>(this Control control, int spacing, IReadOnlyList<T> controls) where T : Control
        {
            if (controls is null)
                throw new ArgumentNullException(nameof(controls));

            List<T[]> result = new();
            int count = 0;
            while (count < controls.Count)
            {
                T[] page = control.FillDownLayout(spacing, controls, count);
                result.Add(page);
                count += page.Length;
            }
            return result.ToArray();
        }
    }
}
