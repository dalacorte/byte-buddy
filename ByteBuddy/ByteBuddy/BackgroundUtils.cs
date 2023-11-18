using static ByteBuddy.BitmapUtils;

namespace ByteBuddy
{
    public static class BackgroundUtils
    {
        public static void SetBackground(Bitmap bitmap, nint handle)
        {
            Handle(bitmap, handle);
        }

        public static void SetBackground(Bitmap bitmap, Control control)
        {
            control.Width = bitmap.Width;
            control.Height = bitmap.Height;

            Handle(bitmap, control.Handle, control);
        }

        private static void Handle(Bitmap bitmap, nint handle, Control? control = null)
        {
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("Format not supported");
            }

            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));

                using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(hBitmap))
                {
                    oldBitmap = Win32.SelectObject(memDc, hBitmapHandle.DangerousGetHandle());
                    Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);

                    int left = 0;
                    int top = 0;

                    if (control is not null)
                    {
                        left = control.Left;
                        top = control.Top;
                    }

                    Win32.Point pointSource = new Win32.Point(left, top);
                    Win32.Point topPos = new Win32.Point(0, 0);
                    Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                    blend.BlendOp = 0;
                    blend.BlendFlags = 0;
                    blend.SourceConstantAlpha = byte.MaxValue;
                    blend.AlphaFormat = 1;
                    Win32.UpdateLayeredWindow(handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, 2);
                }
            }
            //catch (Exception)
            //{
            //    throw;
            //}
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);

                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    Win32.DeleteObject(hBitmap);
                }

                Win32.DeleteDC(memDc);
            }
        }

        // LEGACY

        //IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
        //IntPtr hBitmap = IntPtr.Zero;
        //IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

        //try
        //{
        //    Win32.Point topLoc = new Win32.Point(Left, Top);
        //    Win32.Size bitMapSize = new Win32.Size(frameWidth, frameHeight);
        //    Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
        //    Win32.Point srcLoc = new Win32.Point(0, 0);

        //    hBitmap = GetHBitmap();

        //    using (SafeHBitmapHandle hBitmapHandle = new SafeHBitmapHandle(hBitmap))
        //    {
        //        IntPtr oldBits = Win32.SelectObject(memDc, hBitmapHandle.DangerousGetHandle());

        //        blendFunc.BlendOp = Win32.AC_SRC_OVER;
        //        blendFunc.SourceConstantAlpha = 255;
        //        blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
        //        blendFunc.BlendFlags = 0;

        //        Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
        //    }
        //}
        //finally
        //{
        //    if (hBitmap != IntPtr.Zero)
        //    {
        //        Win32.DeleteObject(hBitmap);
        //    }
        //    Win32.ReleaseDC(IntPtr.Zero, screenDC);
        //    Win32.DeleteDC(memDc);
        //    GC.Collect();
        //}
    }
}
