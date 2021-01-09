using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Screenshots.Adorner
{
    public class ResizeAdorner : System.Windows.Documents.Adorner
    {
        private Handle _currentHandle;
        protected List<Handle> Handles;

        private bool _supportsResize;
        public bool SupportsResize
        {
            get
            {
                return _supportsResize;
            }
            set
            {
                if (value != _supportsResize)
                {
                    _supportsResize = value;
                    InvalidateVisual();
                }
            }
        }

        public static readonly DependencyProperty ShowHandlesProperty = DependencyProperty.Register("ShowHandles", typeof(bool), typeof(ResizeAdorner), new PropertyMetadata(true, ShowHandlesChangedCallback));

        public bool ShowHandles
        {
            get
            {
                return (bool)GetValue(ShowHandlesProperty);
            }
            set
            {
                SetValue(ShowHandlesProperty, value);
            }
        }

        private static void ShowHandlesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResizeAdorner adorner = d as ResizeAdorner;
            adorner.InvalidateVisual();
        }


        public ResizeAdorner(UIElement element)
            : base(element)
        {
            Handles = new List<Handle>();
            base.IsHitTestVisible = true;
            SupportsResize = true;
            ShowHandles = true;
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (_currentHandle != null)
            {
                OnHandleMouseDown?.Invoke(_currentHandle);
                e.Handled = true;
                return;
            }

            e.Handled = false;
            base.OnMouseDown(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ShowHandles)
            {
                Point point = e.MouseDevice.GetPosition(this);
                foreach (Handle handle in Handles)
                {
                    if (handle.MouseTarget.Contains(point))
                    {
                        _currentHandle = handle;
                        base.Cursor = GetCursorForLocation(handle.Location);
                        break;
                    }
                }
            }
            else
            {
                base.Cursor = Cursors.Arrow;
            }

            base.OnMouseMove(e);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect dRect = CalculateHandleLocations();
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(4, 134, 254)), 1.0);
            drawingContext.DrawRectangle(null, pen, dRect);
            if (SupportsResize)
            {
                foreach (Handle handle in Handles)
                {
                    drawingContext.DrawRectangle(Brushes.White, pen, handle.Rect);
                }
            }
        }

        private Rect CalculateHandleLocations()
        {
            Rect Rect = new Rect(base.AdornedElement.DesiredSize);

            float num2 = 7f;
            Size dSize = new Size(num2, num2);
            Point pointB = new Point(num2 / 2f, num2 / 2f);
            Handles.Clear();

            if (ShowHandles)
            {
                Handles.Add(new Handle(new Rect(new Point(Rect.TopLeft.X - pointB.X, Rect.TopLeft.Y - pointB.Y), dSize), ThumbLocation.TopLeft));
                Handles.Add(new Handle(new Rect(new Point(Rect.TopRight.X - pointB.X, Rect.TopRight.Y - pointB.Y), dSize), ThumbLocation.TopRight));
                Handles.Add(new Handle(new Rect(new Point(Rect.BottomLeft.X - pointB.X, Rect.BottomLeft.Y - pointB.Y), dSize), ThumbLocation.BottomLeft));
                Handles.Add(new Handle(new Rect(new Point(Rect.BottomRight.X - pointB.X, Rect.BottomRight.Y - pointB.Y), dSize), ThumbLocation.BottomRight));


                var center = new Point(Rect.Left + Rect.Width / 2d, Rect.Top + Rect.Height / 2d);

                Handles.Add(new Handle(new Rect(new Point(Rect.TopLeft.X - pointB.X, center.Y - pointB.Y), dSize), ThumbLocation.LeftCenter));
                Handles.Add(new Handle(new Rect(new Point(Rect.TopRight.X - pointB.X, center.Y - pointB.Y), dSize), ThumbLocation.RightCenter));
                Handles.Add(new Handle(new Rect(new Point(center.X - pointB.X, Rect.TopLeft.Y - pointB.Y), dSize), ThumbLocation.TopCenter));
                Handles.Add(new Handle(new Rect(new Point(center.X - pointB.X, Rect.BottomLeft.Y - pointB.Y), dSize), ThumbLocation.BottomCenter));
            }

            return Rect;
        }

        public void OverrideCursor(bool isOverride)
        {
            if (_currentHandle == null) return;

            if (isOverride && ShowHandles)
            {
                Mouse.OverrideCursor = GetCursorForLocation(_currentHandle.Location);
            }
            else
            {
                Mouse.OverrideCursor = null;
            }
        }

        private Cursor GetCursorForLocation(ThumbLocation location)
        {
            switch (location)
            {
                case ThumbLocation.TopLeft:
                case ThumbLocation.BottomRight:
                    return Cursors.SizeNWSE;
                case ThumbLocation.TopRight:
                case ThumbLocation.BottomLeft:
                    return Cursors.SizeNESW;
                case ThumbLocation.TopCenter:
                case ThumbLocation.BottomCenter:
                    return Cursors.SizeNS;
                case ThumbLocation.LeftCenter:
                case ThumbLocation.RightCenter:
                    return Cursors.SizeWE;
                default:
                    throw new ArgumentException("Unknown handle location");
            }
        }

        #region 事件

        public delegate void OnHandleMouseDownDelegate(Handle handle);

        public event OnHandleMouseDownDelegate OnHandleMouseDown;

        #endregion

    }
}
