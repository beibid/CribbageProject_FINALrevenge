using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cribbage_Royale_PROYECTO.Models
{
    public static class AvatarImage
    {
        private const double MaximumSide = 128;
        private const int JpegQuality = 80;

        public static byte[] CreateFromFile(string path)
        {
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.CacheOption = BitmapCacheOption.OnLoad;
            source.UriSource = new Uri(path);
            source.EndInit();

            double scale = MaximumSide / Math.Max(source.PixelWidth, source.PixelHeight);
            TransformedBitmap resized = new TransformedBitmap(source, new ScaleTransform(scale, scale));

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = JpegQuality;
            encoder.Frames.Add(BitmapFrame.Create(resized));

            byte[] image;

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                image = stream.ToArray();
            }

            return image;
        }

        public static void ShowIn(Shape shape, byte[] image)
        {
            if (image != null && image.Length > 0)
            {
                ImageBrush brush = new ImageBrush(LoadImage(image));
                brush.Stretch = Stretch.UniformToFill;
                shape.Fill = brush;
            }
        }

        private static BitmapImage LoadImage(byte[] image)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(image))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze();

            return bitmap;
        }
    }
}