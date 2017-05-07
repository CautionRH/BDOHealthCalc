using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

namespace BDOCalc
{
    static class Utils
    {

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
             using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage retval;
            try
            {
                retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            return retval;
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static String GetHealthPercent(BitmapImage bitmapImage) 
        {
            int avgX = 0;
            Bitmap bmp = Utils.BitmapImage2Bitmap(bitmapImage);
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);
            int width = data.Width;
            unsafe
            {
                byte* ptrSrc = (byte*)data.Scan0;
                int bytes = data.Stride * bmp.Height;
                for (int y = 0; y < data.Height; ++y)
                {
                    for (int x = 0; x < data.Width; ++x)
                    {
                        byte r = ptrSrc[2];
                        byte g = ptrSrc[1];
                        byte b = ptrSrc[0];

                        ptrSrc += 4;
                        if (y == data.Height / 2)
                        {
                            if (r > 120 && g > 100)
                            {
                                avgX++;
                            }
                        }
                    }
                }
            }
            bmp.UnlockBits(data);
            float val = ((float)avgX / (float)data.Width);
            String retVal = "" + val * 100.0;
            return retVal;
        }

    }
}
