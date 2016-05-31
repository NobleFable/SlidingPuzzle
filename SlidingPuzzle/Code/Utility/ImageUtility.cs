using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SlidingPuzzle.Utility
{
    public class ImageUtility
    {

        public static Bitmap GetBitmapSection(Bitmap bitmap, int startX, int startY, int width, int height)
        {
            Rectangle rect = new Rectangle(new System.Drawing.Point(startX, startY), new System.Drawing.Size(width, height));
            Bitmap bmp = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.DrawImage(bitmap, 0, 0, rect, GraphicsUnit.Pixel);
            return bmp;
        }

        public static ImageSource GetImageSourceFromBitmap(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static ImageSource GetImageSourceFromBitmap(Bitmap bitmap, int startX, int startY, int width, int height)
        {
            Bitmap bmp = ImageUtility.GetBitmapSection(bitmap, startX, startY, width, height);
            ImageSource imgSource = GetImageSourceFromBitmap(bmp);
            return imgSource;
        }
    }
}
