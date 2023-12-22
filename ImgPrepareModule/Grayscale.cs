using System.Drawing.Imaging;

namespace Лаба_5.ImgPrepareModule
{
    public class Grayscale
    {
        public Bitmap ConvertToGrayscale(Bitmap image)
        {
            // Реализация преобразования изображения в полутоновое
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    // Получаем цвет текущего пикселя
                    Color pixelColor = image.GetPixel(i, j);
                    // Вычисляем среднее значение компонент цвета и устанавливаем его для нового пикселя
                    int grayScale = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                    image.SetPixel(i, j, grayColor);
                }
            }

            return image;
        }

    }
}
