using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Screenshots
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = WinApi.GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            WinApi.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public struct WINDOWPLACEMENT
        {
            public int length;
            public uint flags;
            public uint showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        public static WINDOWPLACEMENT GetPlacement(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, (int)nIndex);
            else
                return GetWindowLongPtr32(hWnd, (int)nIndex);
        }

        public enum GWL
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        /// <summary>
        /// check if windows visible
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);
        
        public static ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumDesktopWindows(IntPtr.Zero, callBackPtr, windowHandles);

            return windowHandles;
        }

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            if (IsWindow(windowHandle) && IsWindowVisible(windowHandle) && !IsIconic(windowHandle))
            {
                windowHandles.Add(windowHandle);
            }
           
            return true;
        }


        /// <summary>
        /// enumarator on all desktop windows
        /// </summary>
        /// <param name="hDesktop"></param>
        /// <param name="lpEnumCallbackFunction"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumedWindow lpEnumCallbackFunction,ArrayList lParam);


        #region 分辨率
        [DllImport("User32.dll")]
        public static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        [DllImport("Shcore.dll")]
        public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, out uint dpiX, out uint dpiY);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Locate_DevNodeA(ref int pdnDevInst, string pDeviceID, int ulFlags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Get_Parent(out UInt32 pdnDevInst, UInt32 dnDevInst, int ulFlags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Get_Device_ID_Size(out int pulLen, UInt32 dnDevInst, int flags = 0);
        [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
        public static extern int CM_Get_Device_ID(UInt32 dnDevInst, char[] buffer, int bufferLen, int flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern int CM_Get_Child(ref int pdnDevInst, int dnDevInst, int ulFlags);

        /// <summary>
        /// 获取设备父系
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool TryGetDriverIdParent(string driver, out string resultDeviceID)
        {
            resultDeviceID = "";
            try
            {
                int CM_LOCATE_DEVNODE_NORMAL = 0x00000000;
                int CR_SUCCESS = 0x00000000;
                UInt32 parentInst;
                int curInst = 0;
                int pLen = 0;
                int apiResult = CM_Locate_DevNodeA(ref curInst, driver, CM_LOCATE_DEVNODE_NORMAL);
                if (apiResult != CR_SUCCESS)
                {
                    return false;
                }
                apiResult = CM_Get_Parent(out parentInst, (UInt32)curInst, CM_LOCATE_DEVNODE_NORMAL);
                if (apiResult != CR_SUCCESS)
                {
                    return false;
                }
                apiResult = CM_Get_Device_ID_Size(out pLen, parentInst, CM_LOCATE_DEVNODE_NORMAL);
                if (apiResult != CR_SUCCESS)
                {
                    return false;
                }
                char[] ptrInstanceBuf = new char[pLen];
                //获取设备id字符串地址
                apiResult = CM_Get_Device_ID(parentInst, ptrInstanceBuf, pLen, 0);
                if (apiResult != CR_SUCCESS)
                {
                    return false;
                }
                resultDeviceID = new string(ptrInstanceBuf);
                return true;
            }
            catch (Exception ecException)
            {
                return false;
            }
        }
        #endregion
    }
}
