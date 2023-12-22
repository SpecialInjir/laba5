namespace Лаба_5.ImgPrepareModule
{
    public class Mininum
    {
        private int _windowSize = 0;

        public Mininum() { }
        public Mininum(int windowSize) { _windowSize = windowSize; }

        public Bitmap Minim(Bitmap original)
        {
            Bitmap result = new(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    // Получаем окрестность текущего пикселя
                    Color[] neighborhood = GetPixelNeighborhood(original, x, y, _windowSize);

                    // Находим минимальное значение цвета для текущего пикселя
                    int minR = int.MaxValue;
                    int minG = int.MaxValue;
                    int minB = int.MaxValue;

                    foreach (var color in neighborhood)
                    {
                        minR = Math.Min(minR, color.R);
                        minG = Math.Min(minG, color.G);
                        minB = Math.Min(minB, color.B);
                    }

                    // Устанавливаем минимальное значение цвета для текущего пикселя
                    result.SetPixel(x, y, Color.FromArgb(minR, minG, minB));
                }
            }

            return result;
        }


        public Bitmap ApplyMaxFilter(Bitmap image)
        {

            // Реализация применения фильтра "максимум"
            // windowSize в оба метода.
            // Этот параметр определяет размер окна, в котором считается минимальное или максимальное значение для каждого пикселя.
            // Размер окна должен быть нечетным числом.
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            int offset = _windowSize / 2;

            for (int i = offset; i < image.Width - offset; i++)
            {
                for (int j = offset; j < image.Height - offset; j++)
                {
                    int maxR = 0, maxG = 0, maxB = 0;

                    for (int k = -offset; k <= offset; k++)
                    {
                        for (int l = -offset; l <= offset; l++)
                        {
                            Color pixel = image.GetPixel(i + k, j + l);
                            maxR = Math.Max(maxR, pixel.R);
                            maxG = Math.Max(maxG, pixel.G);
                            maxB = Math.Max(maxB, pixel.B);
                        }
                    }

                    newImage.SetPixel(i, j, Color.FromArgb(maxR, maxG, maxB));
                }
            }

            return newImage;
        }


        protected static Color[] GetPixelNeighborhood(Bitmap bitmap, int x, int y, int windowSize)
        {
            int halfWindowSize = windowSize / 2;
            Color[] neighborhood = new Color[windowSize * windowSize];

            int index = 0;

            // Проходим по окрестности текущего пикселя
            for (int i = -halfWindowSize; i <= halfWindowSize; i++)
            {
                for (int j = -halfWindowSize; j <= halfWindowSize; j++)
                {
                    int neighborX = Math.Clamp(x + i, 0, bitmap.Width - 1);
                    int neighborY = Math.Clamp(y + j, 0, bitmap.Height - 1);

                    // Добавляем цвет соседнего пикселя в массив
                    neighborhood[index++] = bitmap.GetPixel(neighborX, neighborY);
                }
            }

            return neighborhood;
        }
    }
}
