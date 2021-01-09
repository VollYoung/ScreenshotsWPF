using System;
using System.Windows;

namespace Screenshots.Adorner
{
    /// <summary>
    /// 装饰器上的8个控制点
    /// </summary>
    [Serializable]
    public class Handle
    {
        private Rect _rect;

        public Rect Rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
                Point point = new Point(5, 5);
                Point dPoint = new Point(_rect.Location.X - point.X, _rect.Location.Y - point.Y);
                Size dSize = new Size(_rect.Size.Width + point.X * 2, _rect.Size.Height + point.Y * 2);
                MouseTarget = new Rect(dPoint, dSize);
            }
        }

        public Rect MouseTarget
        {
            get;
            private set;
        }

        public ThumbLocation Location
        {
            get;
            private set;
        }

        public ResizeMode ResizeMode
        {
            get;
            private set;
        }

        public ThumbResizeType ResizeType
        {
            get;
            private set;
        }

        public Handle(Rect rect, ThumbLocation location)
        {
            Rect = rect;
            Location = location;
            ResizeMode = ResizeMode.SizeAndPosition;

            if (Location == ThumbLocation.RightCenter || Location == ThumbLocation.BottomRight || Location == ThumbLocation.BottomCenter)
            {
                ResizeMode = ResizeMode.SizeOnly;
            }
            ResizeType = ThumbResizeType.Both;
            switch (location)
            {
                case ThumbLocation.TopCenter:
                case ThumbLocation.BottomCenter:
                    ResizeType = ThumbResizeType.YOnly;
                    break;
                case ThumbLocation.LeftCenter:
                case ThumbLocation.RightCenter:
                    ResizeType = ThumbResizeType.XOnly;
                    break;
            }
        }
    }
}
