using ByteBuddy.DLL;
using Microsoft.Win32.SafeHandles;

namespace ByteBuddy.Utils
{
    public static class BitmapUtils
    {
        public class SafeHBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeHBitmapHandle(nint preexistingHandle, bool ownsHandle = true)
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
