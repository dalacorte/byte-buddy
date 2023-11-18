using Microsoft.Win32.SafeHandles;

namespace ByteBuddy
{
    public static class BitmapUtils
    {
        public class SafeHBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeHBitmapHandle(IntPtr preexistingHandle, bool ownsHandle = true)
                : base(ownsHandle)
            {
                SetHandle(preexistingHandle);
            }

            protected override bool ReleaseHandle()
            {
                return Win32.DeleteObject(handle);
            }
        }
    }
}
