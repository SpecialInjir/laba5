using Лаба_5.ImgPrepareModule;

namespace Лаба_5.DatabaseModule
{
    public class HashCalculator
    {
        private const int resizedWidth = 8;

        private static Grayscale grayscaleFilter = new();
        private static Monochrome monochromeFilter = new();

        public static ulong Calculate(Bitmap image)
        {
            Bitmap resizedImage = image.Height == resizedWidth && image.Width == resizedWidth
                ? (Bitmap)image.Clone()
                : new(image, resizedWidth, resizedWidth);

            Bitmap grayImage = grayscaleFilter.ConvertToGrayscale(resizedImage);
            Bitmap monochromeImg = monochromeFilter.ConvertToMonochrome(grayImage);

            ulong hash = GenerateHash(monochromeImg);
            return hash;
        }

        private static ulong GenerateHash(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            ulong hash = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = image.GetPixel(x, y);
                    if (color.R != 255)
                    {
                        hash |= 1;
                    }
                    hash <<= 1;
                }
            }

            return hash;
        }
    }
}
