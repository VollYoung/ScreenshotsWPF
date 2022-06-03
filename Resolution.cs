using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace Screenshots
{
    public class Resolution 
    {
        public float DPI
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public float Downscale
        {
            get;
            set;
        }

        public Resolution()
        {
            try
            {
                if (Application.Current == null || Application.Current.MainWindow == null)
                {
                    SetDefaultResolution();
                    return;
                }

                SetVisualResolution(Application.Current.MainWindow);
            }
            catch
            {
                SetDefaultResolution();
            }
        }

        public Resolution(float dpi)
            : this(dpi, 1f)
        {
        }

        public Resolution(float dpi, float scale)
            : this(dpi, scale, 1f)
        {
        }

        public Resolution(float dpi, float scale, float downScale)
        {
            DPI = dpi;
            Scale = scale;
            Downscale = downScale;
        }

        public Resolution(Visual visual)
        {
            if (visual == null)
            {
                SetDefaultResolution();
                return;
            }

            SetVisualResolution(visual);
        }

        private void SetDefaultResolution()
        {
            DPI = GetDpi();
            Scale = 1;
            Downscale = 1f;
        }

        private void SetVisualResolution(Visual visual)
        {
            var dpi = VisualTreeHelper.GetDpi(visual);

            DPI = GetDpi();
            Scale = Convert.ToSingle(dpi.DpiScaleX);
            Downscale = 1f;
        }

        private float GetDpi()
        {
            try
            {
                Screen screen = Screen.AllScreens.First();
                WinApi.GetDpiForMonitor(WinApi.MonitorFromPoint(new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1), 2u), DpiType.Raw, out uint dpiX, out uint dpiY);
                return dpiX;
            }
            catch
            {
                return 96;
            }
        }

        public static Resolution Default { get; } = new Resolution();
    }
}
