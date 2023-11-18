using System.Runtime.InteropServices;
using ByteBuddy.DLL;

namespace ByteBuddy.Utils
{
    public enum TaskbarPosition
    {
        Unknown = -1,
        Left,
        Top,
        Right,
        Bottom,
    }

    public enum ABM : uint
    {
        New = 0x00000000,
        Remove = 0x00000001,
        QueryPos = 0x00000002,
        SetPos = 0x00000003,
        GetState = 0x00000004,
        GetTaskbarPos = 0x00000005,
        Activate = 0x00000006,
        GetAutoHideBar = 0x00000007,
        SetAutoHideBar = 0x00000008,
        WindowPosChanged = 0x00000009,
        SetState = 0x0000000A,
    }

    public enum ABE : uint
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    public static class ABS
    {
        public const int Autohide = 0x0000001;
        public const int AlwaysOnTop = 0x0000002;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public uint cbSize;
        public nint hWnd;
        public uint uCallbackMessage;
        public ABE uEdge;
        public RECT rc;
        public int lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public class TaskbarUtils
    {
        private const string _className = "Shell_TrayWnd";

        public Rectangle Bounds
        {
            get;
            private set;
        }

        public TaskbarPosition Position
        {
            get;
            private set;
        }

        public Point Location
        {
            get
            {
                return Bounds.Location;
            }
        }

        public Size Size
        {
            get
            {
                return Bounds.Size;
            }
        }

        //Always returns false under Windows 7
        public bool AlwaysOnTop
        {
            get;
            private set;
        }
        public bool AutoHide
        {
            get;
            private set;
        }

        public TaskbarUtils()
        {
            nint taskbarHandle = Win32.FindWindow(_className, string.Empty);

            APPBARDATA data = new APPBARDATA();
            data.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
            data.hWnd = taskbarHandle;
            nint result = Win32.SHAppBarMessage(ABM.GetTaskbarPos, ref data);
            if (result == nint.Zero)
                throw new InvalidOperationException();

            Position = (TaskbarPosition)data.uEdge;
            Bounds = Rectangle.FromLTRB(data.rc.left, data.rc.top, data.rc.right, data.rc.bottom);

            data.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
            result = Win32.SHAppBarMessage(ABM.GetState, ref data);
            int state = result.ToInt32();
            AlwaysOnTop = (state & ABS.AlwaysOnTop) == ABS.AlwaysOnTop;
            AutoHide = (state & ABS.Autohide) == ABS.Autohide;
        }

        public bool IsWindowsVisible()
        {
            IntPtr hWnd = Win32.FindWindow("Shell_TrayWnd", string.Empty);
            return Win32.IsWindowVisible(hWnd);
        }
    }
}
