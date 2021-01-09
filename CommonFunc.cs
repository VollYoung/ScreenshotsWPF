using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Screenshots
{
    public class CommonFunc
    {
        /// <summary>
        /// 获取靠近Element的位置
        /// </summary>
        /// <param name="closetoElement">被靠近的Element</param>
        /// <param name="targetSize">要靠近的目标大小</param>
        /// <param name="parent">相对于的父级</param>
        public static Point CalcClosetoLocation(UIElement closetoElement, Size targetSize, UIElement parent)
        {
            var bounds = closetoElement.TransformToVisual(parent).TransformBounds(new Rect(closetoElement.RenderSize));

            if (bounds.Size.Width > bounds.Size.Height)
            {
                var maxX = parent.RenderSize.Width - targetSize.Width;
                var x = Convert.ToSingle(Math.Max(0, Math.Min(bounds.Center().X - targetSize.Width / 2, maxX)));

                if (bounds.Center().Y > parent.RenderSize.Height / 2)
                {
                    //top
                    return new Point(x, bounds.Top - targetSize.Height);
                }
                else
                {
                    //bottom
                    return new Point(x, bounds.Bottom);
                }
            }
            else
            {
                var maxY = parent.RenderSize.Height - targetSize.Height;
                var y = Convert.ToSingle(Math.Max(0, Math.Min(bounds.Center().Y - targetSize.Height / 2, maxY)));

                if (bounds.Center().X > parent.RenderSize.Width / 2)
                {
                    //left
                    return new Point(bounds.Left - targetSize.Width, y);
                }
                else
                {
                    //right
                    return new Point(bounds.Right, y);
                }
            }
        }
    }
}
