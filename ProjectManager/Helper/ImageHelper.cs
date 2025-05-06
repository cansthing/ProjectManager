using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace ProjectManager.Helper
{
    public class ImageHelper
    {
        public static ImageSource ConvertToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
        public static byte[] ImageSourceToBinary(ImageSource imageSource)
        {
            if (imageSource == null)
                return null;

            BitmapSource bitmapSource = imageSource as BitmapSource;
            if (bitmapSource == null)
                return null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("ImageToBinary", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

    }
}
