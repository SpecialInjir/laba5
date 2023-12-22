namespace Лаба_5.ImgPrepareModule
{
    public class ImagePreparator
    {

        public int MatrixSize
        {
            get => matrixSize;
            set
            {
                matrixSize = value;
                _mininumFilter = new(matrixSize);
            }
        }

        public int MonochromeBound
        {
            get => monochromeBound;
            set
            {
                monochromeBound = value;
                _monochromeFilter = new(monochromeBound);
            }
        }

        private Monochrome _monochromeFilter = new();
        private Grayscale _grayscaleFilter = new();
        private Mininum _mininumFilter = new();

        private int matrixSize;
        private int kernelSize = 3;
        private int monochromeBound;

        public ImagePreparator(int matrixSize, int monochromeBound)
        {
            MatrixSize = matrixSize;

            _mininumFilter = new(matrixSize);
            _monochromeFilter = new(monochromeBound);

            MatrixSize = matrixSize;
        }

        public Bitmap ImgPrepare(Bitmap bitmap)
        {
            Bitmap convertedBitmap = (Bitmap)bitmap.Clone();

            convertedBitmap = _grayscaleFilter.ConvertToGrayscale(convertedBitmap);
            convertedBitmap = _mininumFilter.Minim(convertedBitmap);
            convertedBitmap = _monochromeFilter.ConvertToMonochrome(convertedBitmap);
            return MorphologicalDilationF(convertedBitmap);
        }

        public Bitmap ConvertToGrayscale(Bitmap original)
        {
            return _grayscaleFilter.ConvertToGrayscale(original);
        }

        public Bitmap ConvertToMonochrome(Bitmap original)
        {
            return _monochromeFilter.ConvertToMonochrome(original);
        }

        public Bitmap GrayscaleTransform(Bitmap original)
        {
            return _mininumFilter.Minim(original);
        }

        public Bitmap MorphologicalDilationF(Bitmap original)
        {
            // Реализация морфологических преобразований(дилатации) над монохромным изображением

            Bitmap result = new(original.Width, original.Height);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    // Применяем структурный элемент (квадрат)
                    for (int i = -kernelSize / 2; i <= kernelSize / 2; i++)
                    {
                        for (int j = -kernelSize / 2; j <= kernelSize / 2; j++)
                        {
                            int neighborX = Math.Clamp(x + i, 0, original.Width - 1);
                            int neighborY = Math.Clamp(y + j, 0, original.Height - 1);

                            if (original.GetPixel(neighborX, neighborY).R == 0)
                            {
                                result.SetPixel(x, y, Color.Black);
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}

