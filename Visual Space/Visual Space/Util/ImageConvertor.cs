using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Nollan.Visual_Space.Util
{
    static class ImageConvertor
    {

        private static System.Drawing.Bitmap ConvertToBitmap(BitmapSource bitmapSource)
        {


            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, memoryBlockPointer);
            return bitmap;
        }

        public static byte[] ConvertImageBrushToJpegByteArray(ImageBrush imageBrush)
        {

            if (imageBrush == null)
                return null;

            BitmapSource bs = imageBrush.ImageSource as BitmapSource;
            System.Drawing.Bitmap bmp = ConvertToBitmap(bs);
            Byte[] data;

            using (var memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                data = memoryStream.ToArray();
            }

            return data;

        }

    }
}
