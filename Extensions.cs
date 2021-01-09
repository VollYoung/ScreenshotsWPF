using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Screenshots
{
    public static class Extensions
    {
        public static Point Center(this Rect rect)
        {
            var x = rect.Left + rect.Width / 2f;
            var y = rect.Top + rect.Height / 2f;
            return new Point(x, y);
        }
    }
}
