using System.Drawing.Imaging;

namespace Лаба_5.ImgPrepareModule
{
    public class Monochrome
    {
        private int _threshold = 128;

        public Monochrome() { }

        public Monochrome(int threshold) { _threshold = threshold; }

        public Bitmap ConvertToMonochrome(Bitmap img)
        {
            Bitmap monoImage = new(img.Width, img.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color pixelColor = img.GetPixel(x, y);

                    // Вычисляем яркость пикселя
                    int brightness = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);

                    // Устанавливаем цвет пикселя в зависимости от яркости и порога
                    Color monoImageColor = brightness > _threshold ? Color.White : Color.Black;
                    monoImage.SetPixel(x, y, monoImageColor);
                }
            }

            return monoImage;
        }

    }
}
