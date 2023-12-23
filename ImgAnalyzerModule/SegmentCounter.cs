//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using global::AForge.Imaging;
//using global::AForge;
//using Point = AForge.Point;

//namespace Лаба_5.ImgAnalyzerModule
//{
//        public  class SegmentCounter
//    {

//            public Bitmap FindConnectedComponents(Bitmap binaryBmp)
//            {
//                int label = 0;
//                Bitmap negatedBmp = NegateImage(binaryBmp);

//                for (int x = 0; x < negatedBmp.Width; x++)
//                {
//                    for (int y = 0; y < negatedBmp.Height; y++)
//                    {
//                        if (GetPixelValue(negatedBmp, x, y) == -1)
//                        {
//                            label++;
//                            SearchConnectedComponent(negatedBmp, label, x, y);
//                        }
//                    }
//                }

//                return negatedBmp;
//            }

//            private Bitmap NegateImage(Bitmap bmp)
//            {
//                Bitmap result = new Bitmap(bmp.Width, bmp.Height);
//                for (int x = 0; x < bmp.Width; x++)
//                {
//                    for (int y = 0; y < bmp.Height; y++)
//                    {
//                        Color pixel = bmp.GetPixel(x, y);
//                        Color newPixel = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
//                        result.SetPixel(x, y, newPixel);
//                    }
//                }
//                return result;
//            }

//            private List<Point> GetNeighbors(Bitmap bmp, int x, int y)
//            {
//                List<Point> neighbors = new List<Point>();
//                // реализация получения соседних пикселей
//                return neighbors;
//            }

//            private int GetPixelValue(Bitmap bmp, int x, int y)
//            {
//                // Возвращает значение пикселя изображения по указанным координатам
//                return -1;
//            }

//            private void SetPixelValue(Bitmap bmp, int x, int y, int value)
//            {
//                // Устанавливает значение пикселя изображения по указанным координатам
//            }

//            private void SearchConnectedComponent(Bitmap bmp, int label, int x, int y)
//            {
//                SetPixelValue(bmp, x, y, label);
//                List<Point> neighbors = GetNeighbors(bmp, x, y);

//                foreach (Point neighbor in neighbors)
//                {
//                    if (GetPixelValue(bmp, (int)neighbor.X, (int)neighbor.Y) == -1)
//                    {
//                        SearchConnectedComponent(bmp, label, (int)neighbor.X, (int)neighbor.Y);
//                    }
//                }
//            }

  
//        }

//    }

