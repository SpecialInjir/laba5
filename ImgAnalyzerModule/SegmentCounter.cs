using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using global::AForge.Imaging;
using global::AForge;

namespace Лаба_5.ImgAnalyzerModule
{
        public abstract class SegmentCounter
    {
            private class BlobsSorter : IComparer<Blob>
            {
                private ObjectsOrder order;

                public BlobsSorter(ObjectsOrder order)
                {
                    this.order = order;
                }

                public int Compare(Blob a, Blob b)
                {
                    Rectangle rectangle = a.Rectangle;
                    Rectangle rectangle2 = b.Rectangle;
                    return order switch
                    {
                        ObjectsOrder.Size => rectangle2.Width * rectangle2.Height - rectangle.Width * rectangle.Height,
                        ObjectsOrder.Area => b.Area - a.Area,
                        ObjectsOrder.YX => rectangle.Y * 100000 + rectangle.X - (rectangle2.Y * 100000 + rectangle2.X),
                        ObjectsOrder.XY => rectangle.X * 100000 + rectangle.Y - (rectangle2.X * 100000 + rectangle2.Y),
                        _ => 0,
                    };
                }
            }

            private List<Blob> blobs = new List<Blob>();

            private ObjectsOrder objectsOrder;

            private bool filterBlobs;

            private IBlobsFilter filter;

            private bool coupledSizeFiltering;

            private int minWidth = 1;

            private int minHeight = 1;

            private int maxWidth = int.MaxValue;

            private int maxHeight = int.MaxValue;

            //
            // Сводка:
            //     Objects count.
            protected int objectsCount;

            //
            // Сводка:
            //     Objects' labels.
            protected int[] objectLabels;

            //
            // Сводка:
            //     Width of processed image.
            protected int imageWidth;

            //
            // Сводка:
            //     Height of processed image.
            protected int imageHeight;

            //
            // Сводка:
            //     Objects count.
            //
            // Примечания:
            //     Number of objects (blobs) found by AForge.Imaging.BlobCounterBase.ProcessImage(System.Drawing.Bitmap)
            //     method.
            public int ObjectsCount => objectsCount;

            //
            // Сводка:
            //     Objects' labels.
            //
            // Примечания:
            //     The array of width * height size, which holds labels for all objects. Background
            //     is represented with 0 value, but objects are represented with labels starting
            //     from 1.
            public int[] ObjectLabels => objectLabels;

            //
            // Сводка:
            //     Objects sort order.
            //
            // Примечания:
            //     The property specifies objects' sort order, which are provided by AForge.Imaging.BlobCounterBase.GetObjectsRectangles,
            //     AForge.Imaging.BlobCounterBase.GetObjectsInformation, etc.
            public ObjectsOrder ObjectsOrder
            {
                get
                {
                    return objectsOrder;
                }
                set
                {
                    objectsOrder = value;
                }
            }

            //
            // Сводка:
            //     Specifies if blobs should be filtered.
            //
            // Примечания:
            //     If the property is equal to false, then there is no any additional post processing
            //     after image was processed. If the property is set to true, then blobs filtering
            //     is done right after image processing routine. If AForge.Imaging.BlobCounterBase.BlobsFilter
            //     is set, then custom blobs' filtering is done, which is implemented by user. Otherwise
            //     blobs are filtered according to dimensions specified in AForge.Imaging.BlobCounterBase.MinWidth,
            //     AForge.Imaging.BlobCounterBase.MinHeight, AForge.Imaging.BlobCounterBase.MaxWidth
            //     and AForge.Imaging.BlobCounterBase.MaxHeight properties.
            //     Default value is set to false.
            public bool FilterBlobs
            {
                get
                {
                    return filterBlobs;
                }
                set
                {
                    filterBlobs = value;
                }
            }

            //
            // Сводка:
            //     Specifies if size filetering should be coupled or not.
            //
            // Примечания:
            //     In uncoupled filtering mode, objects are filtered out in the case if their width
            //     is smaller than AForge.Imaging.BlobCounterBase.MinWidth or height is smaller
            //     than AForge.Imaging.BlobCounterBase.MinHeight. But in coupled filtering mode,
            //     objects are filtered out in the case if their width is smaller than AForge.Imaging.BlobCounterBase.MinWidth
            //     and height is smaller than AForge.Imaging.BlobCounterBase.MinHeight. In both
            //     modes the idea with filtering by objects' maximum size is the same as filtering
            //     by objects' minimum size.
            //     Default value is set to false, what means uncoupled filtering by size.
            public bool CoupledSizeFiltering
            {
                get
                {
                    return coupledSizeFiltering;
                }
                set
                {
                    coupledSizeFiltering = value;
                }
            }

            //
            // Сводка:
            //     Minimum allowed width of blob.
            //
            // Примечания:
            //     The property specifies minimum object's width acceptable by blob counting routine
            //     and has power only when AForge.Imaging.BlobCounterBase.FilterBlobs property is
            //     set to true and custom blobs' filter is set to null.
            //     See documentation to AForge.Imaging.BlobCounterBase.CoupledSizeFiltering for
            //     additional information.
            public int MinWidth
            {
                get
                {
                    return minWidth;
                }
                set
                {
                    minWidth = value;
                }
            }

            //
            // Сводка:
            //     Minimum allowed height of blob.
            //
            // Примечания:
            //     The property specifies minimum object's height acceptable by blob counting routine
            //     and has power only when AForge.Imaging.BlobCounterBase.FilterBlobs property is
            //     set to true and custom blobs' filter is set to null.
            //     See documentation to AForge.Imaging.BlobCounterBase.CoupledSizeFiltering for
            //     additional information.
            public int MinHeight
            {
                get
                {
                    return minHeight;
                }
                set
                {
                    minHeight = value;
                }
            }

            //
            // Сводка:
            //     Maximum allowed width of blob.
            //
            // Примечания:
            //     The property specifies maximum object's width acceptable by blob counting routine
            //     and has power only when AForge.Imaging.BlobCounterBase.FilterBlobs property is
            //     set to true and custom blobs' filter is set to null.
            //     See documentation to AForge.Imaging.BlobCounterBase.CoupledSizeFiltering for
            //     additional information.
            public int MaxWidth
            {
                get
                {
                    return maxWidth;
                }
                set
                {
                    maxWidth = value;
                }
            }

            //
            // Сводка:
            //     Maximum allowed height of blob.
            //
            // Примечания:
            //     The property specifies maximum object's height acceptable by blob counting routine
            //     and has power only when AForge.Imaging.BlobCounterBase.FilterBlobs property is
            //     set to true and custom blobs' filter is set to null.
            //     See documentation to AForge.Imaging.BlobCounterBase.CoupledSizeFiltering for
            //     additional information.
            public int MaxHeight
            {
                get
                {
                    return maxHeight;
                }
                set
                {
                    maxHeight = value;
                }
            }

            //
            // Сводка:
            //     Custom blobs' filter to use.
            //
            // Примечания:
            //     The property specifies custom blobs' filtering routine to use. It has effect
            //     only in the case if AForge.Imaging.BlobCounterBase.FilterBlobs property is set
            //     to true.
            //     When custom blobs' filtering routine is set, it has priority over default filtering
            //     done with AForge.Imaging.BlobCounterBase.MinWidth, AForge.Imaging.BlobCounterBase.MinHeight,
            //     AForge.Imaging.BlobCounterBase.MaxWidth and AForge.Imaging.BlobCounterBase.MaxHeight.
            public IBlobsFilter BlobsFilter
            {
                get
                {
                    return filter;
                }
                set
                {
                    filter = value;
                }
            }



            public void ProcessImage(Bitmap image)
            {
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                try
                {
                    ProcessImage(bitmapData);
                }
                finally
                {
                    image.UnlockBits(bitmapData);
                }
            }

            public void ProcessImage(BitmapData imageData)
            {
                ProcessImage(new UnmanagedImage(imageData));
            }

            public void ProcessImage(UnmanagedImage image)
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
                BuildObjectsMap(image);
                CollectObjectsInfo(image);
                if (filterBlobs)
                {
                    int[] array = new int[objectsCount + 1];
                    for (int i = 1; i <= objectsCount; i++)
                    {
                        array[i] = i;
                    }

                    int num = 0;
                    if (filter == null)
                    {
                        for (int num2 = objectsCount - 1; num2 >= 0; num2--)
                        {
                            int width = blobs[num2].Rectangle.Width;
                            int height = blobs[num2].Rectangle.Height;
                            if (!coupledSizeFiltering)
                            {
                                if (width < minWidth || height < minHeight || width > maxWidth || height > maxHeight)
                                {
                                    array[num2 + 1] = 0;
                                    num++;
                                    blobs.RemoveAt(num2);
                                }
                            }
                            else if ((width < minWidth && height < minHeight) || (width > maxWidth && height > maxHeight))
                            {
                                array[num2 + 1] = 0;
                                num++;
                                blobs.RemoveAt(num2);
                            }
                        }
                    }
                    else
                    {
                        for (int num3 = objectsCount - 1; num3 >= 0; num3--)
                        {
                            if (!filter.Check(blobs[num3]))
                            {
                                array[num3 + 1] = 0;
                                num++;
                                blobs.RemoveAt(num3);
                            }
                        }
                    }

                    int num4 = 0;
                    for (int j = 1; j <= objectsCount; j++)
                    {
                        if (array[j] != 0)
                        {
                            num4 = (array[j] = num4 + 1);
                        }
                    }

                    int k = 0;
                    for (int num5 = objectLabels.Length; k < num5; k++)
                    {
                        objectLabels[k] = array[objectLabels[k]];
                    }

                    objectsCount -= num;
                    int l = 0;
                    for (int count = blobs.Count; l < count; l++)
                    {
                        blobs[l].ID = l + 1;
                    }
                }

                if (objectsOrder != 0)
                {
                    blobs.Sort(new BlobsSorter(objectsOrder));
                }
            }

            public Rectangle[] GetObjectsRectangles()
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                Rectangle[] array = new Rectangle[objectsCount];
                for (int i = 0; i < objectsCount; i++)
                {
                    ref Rectangle reference = ref array[i];
                    reference = blobs[i].Rectangle;
                }

                return array;
            }

        
            public Blob[] GetObjectsInformation()
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                Blob[] array = new Blob[objectsCount];
                for (int i = 0; i < objectsCount; i++)
                {
                    array[i] = new Blob(blobs[i]);
                }

                return array;
            }

          
            public Blob[] GetObjects(Bitmap image, bool extractInOriginalSize)
            {
                Blob[] array = null;
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                try
                {
                    return GetObjects(new UnmanagedImage(bitmapData), extractInOriginalSize);
                }
                finally
                {
                    image.UnlockBits(bitmapData);
                }
            }

          
            public unsafe Blob[] GetObjects(UnmanagedImage image, bool extractInOriginalSize)
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                if (image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
                {
                    throw new UnsupportedImageFormatException("Unsupported pixel format of the provided image.");
                }

                int width = image.Width;
                int height = image.Height;
                int stride = image.Stride;
                int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
                Blob[] array = new Blob[objectsCount];
                for (int i = 0; i < objectsCount; i++)
                {
                    int width2 = blobs[i].Rectangle.Width;
                    int height2 = blobs[i].Rectangle.Height;
                    int width3 = (extractInOriginalSize ? width : width2);
                    int height3 = (extractInOriginalSize ? height : height2);
                    int x = blobs[i].Rectangle.X;
                    int num2 = x + width2 - 1;
                    int y = blobs[i].Rectangle.Y;
                    int num3 = y + height2 - 1;
                    int iD = blobs[i].ID;
                    UnmanagedImage unmanagedImage = UnmanagedImage.Create(width3, height3, image.PixelFormat);
                    byte* ptr = (byte*)image.ImageData.ToPointer() + (nint)y * (nint)stride + (nint)x * (nint)num;
                    byte* ptr2 = (byte*)unmanagedImage.ImageData.ToPointer();
                    int num4 = y * width + x;
                    if (extractInOriginalSize)
                    {
                        ptr2 += y * unmanagedImage.Stride + x * num;
                    }

                    int num5 = stride - width2 * num;
                    int num6 = unmanagedImage.Stride - width2 * num;
                    int num7 = width - width2;
                    for (int j = y; j <= num3; j++)
                    {
                        int num8 = x;
                        while (num8 <= num2)
                        {
                            if (objectLabels[num4] == iD)
                            {
                                *ptr2 = *ptr;
                                if (num > 1)
                                {
                                    ptr2[1] = ptr[1];
                                    ptr2[2] = ptr[2];
                                    if (num > 3)
                                    {
                                        ptr2[3] = ptr[3];
                                    }
                                }
                            }

                            num8++;
                            num4++;
                            ptr2 += num;
                            ptr += num;
                        }

                        ptr += num5;
                        ptr2 += num6;
                        num4 += num7;
                    }

                    array[i] = new Blob(blobs[i]);
                    array[i].Image = unmanagedImage;
                    array[i].OriginalSize = extractInOriginalSize;
                }

                return array;
            }

            //
            // Сводка:
            //     Extract blob's image.
            //
            // Параметры:
            //   image:
            //     Source image to extract blob's image from.
            //
            //   blob:
            //     Blob which is required to be extracted.
            //
            //   extractInOriginalSize:
            //     Specifies size of blobs' image to extract. If set to true each blobs' image will
            //     have the same size as the specified image. If set to false each blobs' image
            //     will have the size of its blob.
            //
            // Исключения:
            //   T:AForge.Imaging.UnsupportedImageFormatException:
            //     Unsupported pixel format of the provided image.
            //
            //   T:System.ApplicationException:
            //     No image was processed before, so blob can not be extracted.
            //
            // Примечания:
            //     The method is used to extract image of partially initialized blob, which was
            //     provided by AForge.Imaging.BlobCounterBase.GetObjectsInformation method. Before
            //     calling the method, the AForge.Imaging.BlobCounterBase.ProcessImage(System.Drawing.Bitmap),
            //     AForge.Imaging.BlobCounterBase.ProcessImage(System.Drawing.Imaging.BitmapData)
            //     or AForge.Imaging.BlobCounterBase.ProcessImage(AForge.Imaging.UnmanagedImage)
            //     method should be called, which will build objects map.
            //     The method supports 24/32 bpp color and 8 bpp indexed grayscale images.
            public void ExtractBlobsImage(Bitmap image, Blob blob, bool extractInOriginalSize)
            {
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                try
                {
                    ExtractBlobsImage(new UnmanagedImage(bitmapData), blob, extractInOriginalSize);
                }
                finally
                {
                    image.UnlockBits(bitmapData);
                }
            }

            //
            // Сводка:
            //     Extract blob's image.
            //
            // Параметры:
            //   image:
            //     Source unmanaged image to extract blob's image from.
            //
            //   blob:
            //     Blob which is required to be extracted.
            //
            //   extractInOriginalSize:
            //     Specifies size of blobs' image to extract. If set to true each blobs' image will
            //     have the same size as the specified image. If set to false each blobs' image
            //     will have the size of its blob.
            //
            // Исключения:
            //   T:AForge.Imaging.UnsupportedImageFormatException:
            //     Unsupported pixel format of the provided image.
            //
            //   T:System.ApplicationException:
            //     No image was processed before, so blob can not be extracted.
            //
            // Примечания:
            //     The method is used to extract image of partially initialized blob, which was
            //     provided by AForge.Imaging.BlobCounterBase.GetObjectsInformation method. Before
            //     calling the method, the AForge.Imaging.BlobCounterBase.ProcessImage(System.Drawing.Bitmap),
            //     AForge.Imaging.BlobCounterBase.ProcessImage(System.Drawing.Imaging.BitmapData)
            //     or AForge.Imaging.BlobCounterBase.ProcessImage(AForge.Imaging.UnmanagedImage)
            //     method should be called, which will build objects map.
            //     The method supports 24/32 bpp color and 8 bpp indexed grayscale images.
            public unsafe void ExtractBlobsImage(UnmanagedImage image, Blob blob, bool extractInOriginalSize)
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                if (image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
                {
                    throw new UnsupportedImageFormatException("Unsupported pixel format of the provided image.");
                }

                int width = image.Width;
                int height = image.Height;
                int stride = image.Stride;
                int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int width2 = blob.Rectangle.Width;
                int height2 = blob.Rectangle.Height;
                int width3 = (extractInOriginalSize ? width : width2);
                int height3 = (extractInOriginalSize ? height : height2);
                int left = blob.Rectangle.Left;
                int num2 = left + width2 - 1;
                int top = blob.Rectangle.Top;
                int num3 = top + height2 - 1;
                int iD = blob.ID;
                blob.Image = UnmanagedImage.Create(width3, height3, image.PixelFormat);
                blob.OriginalSize = extractInOriginalSize;
                byte* ptr = (byte*)image.ImageData.ToPointer() + (nint)top * (nint)stride + (nint)left * (nint)num;
                byte* ptr2 = (byte*)blob.Image.ImageData.ToPointer();
                int num4 = top * width + left;
                if (extractInOriginalSize)
                {
                    ptr2 += top * blob.Image.Stride + left * num;
                }

                int num5 = stride - width2 * num;
                int num6 = blob.Image.Stride - width2 * num;
                int num7 = width - width2;
                for (int i = top; i <= num3; i++)
                {
                    int num8 = left;
                    while (num8 <= num2)
                    {
                        if (objectLabels[num4] == iD)
                        {
                            *ptr2 = *ptr;
                            if (num > 1)
                            {
                                ptr2[1] = ptr[1];
                                ptr2[2] = ptr[2];
                                if (num > 3)
                                {
                                    ptr2[3] = ptr[3];
                                }
                            }
                        }

                        num8++;
                        num4++;
                        ptr2 += num;
                        ptr += num;
                    }

                    ptr += num5;
                    ptr2 += num6;
                    num4 += num7;
                }
            }

     
            public void GetBlobsLeftAndRightEdges(Blob blob, out List<IntPoint> leftEdge, out List<IntPoint> rightEdge)
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                leftEdge = new List<IntPoint>();
                rightEdge = new List<IntPoint>();
                int left = blob.Rectangle.Left;
                int num = left + blob.Rectangle.Width - 1;
                int top = blob.Rectangle.Top;
                int num2 = top + blob.Rectangle.Height - 1;
                int iD = blob.ID;
                for (int i = top; i <= num2; i++)
                {
                    int num3 = i * imageWidth + left;
                    int num4 = left;
                    while (num4 <= num)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            leftEdge.Add(new IntPoint(num4, i));
                            break;
                        }

                        num4++;
                        num3++;
                    }

                    num3 = i * imageWidth + num;
                    int num5 = num;
                    while (num5 >= left)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            rightEdge.Add(new IntPoint(num5, i));
                            break;
                        }

                        num5--;
                        num3--;
                    }
                }
            }

         
            public void GetBlobsTopAndBottomEdges(Blob blob, out List<IntPoint> topEdge, out List<IntPoint> bottomEdge)
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                topEdge = new List<IntPoint>();
                bottomEdge = new List<IntPoint>();
                int left = blob.Rectangle.Left;
                int num = left + blob.Rectangle.Width - 1;
                int top = blob.Rectangle.Top;
                int num2 = top + blob.Rectangle.Height - 1;
                int iD = blob.ID;
                for (int i = left; i <= num; i++)
                {
                    int num3 = top * imageWidth + i;
                    int num4 = top;
                    while (num4 <= num2)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            topEdge.Add(new IntPoint(i, num4));
                            break;
                        }

                        num4++;
                        num3 += imageWidth;
                    }

                    num3 = num2 * imageWidth + i;
                    int num5 = num2;
                    while (num5 >= top)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            bottomEdge.Add(new IntPoint(i, num5));
                            break;
                        }

                        num5--;
                        num3 -= imageWidth;
                    }
                }
            }

            //
            // Сводка:
            //     Get list of object's edge points.
            //
            // Параметры:
            //   blob:
            //     Blob to collect edge points for.
            //
            // Возврат:
            //     Returns unsorted list of blob's edge points.
            //
            // Исключения:
            //   T:System.ApplicationException:
            //     No image was processed before, so blob can not be extracted.
            //
            // Примечания:
            //     The method scans each row and column of the blob and finds the most top/bottom/left/right
            //     points. The method returns similar result as if results of both AForge.Imaging.BlobCounterBase.GetBlobsLeftAndRightEdges(AForge.Imaging.Blob,System.Collections.Generic.List{AForge.IntPoint}@,System.Collections.Generic.List{AForge.IntPoint}@)
            //     and AForge.Imaging.BlobCounterBase.GetBlobsTopAndBottomEdges(AForge.Imaging.Blob,System.Collections.Generic.List{AForge.IntPoint}@,System.Collections.Generic.List{AForge.IntPoint}@)
            //     methods were combined, but each edge point occurs only once in the list.
            //     Edge points in the returned list are not ordered. This makes the list unusable
            //     for visualization with methods, which draw polygon or poly-line. But the returned
            //     list can be used with such algorithms, like convex hull search, shape analyzer,
            //     etc.
            public List<IntPoint> GetBlobsEdgePoints(Blob blob)
            {
                if (objectLabels == null)
                {
                    throw new ApplicationException("Image should be processed before to collect objects map.");
                }

                List<IntPoint> list = new List<IntPoint>();
                int left = blob.Rectangle.Left;
                int num = left + blob.Rectangle.Width - 1;
                int top = blob.Rectangle.Top;
                int num2 = top + blob.Rectangle.Height - 1;
                int iD = blob.ID;
                int[] array = new int[blob.Rectangle.Height];
                int[] array2 = new int[blob.Rectangle.Height];
                for (int i = top; i <= num2; i++)
                {
                    int num3 = i * imageWidth + left;
                    int num4 = left;
                    while (num4 <= num)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            list.Add(new IntPoint(num4, i));
                            array[i - top] = num4;
                            break;
                        }

                        num4++;
                        num3++;
                    }

                    num3 = i * imageWidth + num;
                    int num5 = num;
                    while (num5 >= left)
                    {
                        if (objectLabels[num3] == iD)
                        {
                            if (array[i - top] != num5)
                            {
                                list.Add(new IntPoint(num5, i));
                            }

                            array2[i - top] = num5;
                            break;
                        }

                        num5--;
                        num3--;
                    }
                }

                for (int j = left; j <= num; j++)
                {
                    int num6 = top * imageWidth + j;
                    int num7 = top;
                    int num8 = 0;
                    while (num7 <= num2)
                    {
                        if (objectLabels[num6] == iD)
                        {
                            if (array[num8] != j && array2[num8] != j)
                            {
                                list.Add(new IntPoint(j, num7));
                            }

                            break;
                        }

                        num7++;
                        num8++;
                        num6 += imageWidth;
                    }

                    num6 = num2 * imageWidth + j;
                    int num9 = num2;
                    int num10 = num2 - top;
                    while (num9 >= top)
                    {
                        if (objectLabels[num6] == iD)
                        {
                            if (array[num10] != j && array2[num10] != j)
                            {
                                list.Add(new IntPoint(j, num9));
                            }

                            break;
                        }

                        num9--;
                        num10--;
                        num6 -= imageWidth;
                    }
                }

                return list;
            }

            //
            // Сводка:
            //     Actual objects map building.
            //
            // Параметры:
            //   image:
            //     Unmanaged image to process.
            //
            // Примечания:
            //     By the time this method is called bitmap's pixel format is not yet checked, so
            //     this should be done by the class inheriting from the base class. AForge.Imaging.BlobCounterBase.imageWidth
            //     and AForge.Imaging.BlobCounterBase.imageHeight members are initialized before
            //     the method is called, so these members may be used safely.
            protected abstract void BuildObjectsMap(UnmanagedImage image);

            private unsafe void CollectObjectsInfo(UnmanagedImage image)
            {
                int num = 0;
                int[] array = new int[objectsCount + 1];
                int[] array2 = new int[objectsCount + 1];
                int[] array3 = new int[objectsCount + 1];
                int[] array4 = new int[objectsCount + 1];
                int[] array5 = new int[objectsCount + 1];
                long[] array6 = new long[objectsCount + 1];
                long[] array7 = new long[objectsCount + 1];
                long[] array8 = new long[objectsCount + 1];
                long[] array9 = new long[objectsCount + 1];
                long[] array10 = new long[objectsCount + 1];
                long[] array11 = new long[objectsCount + 1];
                long[] array12 = new long[objectsCount + 1];
                long[] array13 = new long[objectsCount + 1];
                for (int i = 1; i <= objectsCount; i++)
                {
                    array[i] = imageWidth;
                    array2[i] = imageHeight;
                }

                byte* ptr = (byte*)image.ImageData.ToPointer();
                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    int num2 = image.Stride - imageWidth;
                    for (int j = 0; j < imageHeight; j++)
                    {
                        int num3 = 0;
                        while (num3 < imageWidth)
                        {
                            int num4 = objectLabels[num];
                            if (num4 != 0)
                            {
                                if (num3 < array[num4])
                                {
                                    array[num4] = num3;
                                }

                                if (num3 > array3[num4])
                                {
                                    array3[num4] = num3;
                                }

                                if (j < array2[num4])
                                {
                                    array2[num4] = j;
                                }

                                if (j > array4[num4])
                                {
                                    array4[num4] = j;
                                }

                                array5[num4]++;
                                array6[num4] += num3;
                                array7[num4] += j;
                                byte b = *ptr;
                                array9[num4] += b;
                                array12[num4] += b * b;
                            }

                            num3++;
                            num++;
                            ptr++;
                        }

                        ptr += num2;
                    }

                    for (int k = 1; k <= objectsCount; k++)
                    {
                        array8[k] = (array10[k] = array9[k]);
                        array11[k] = (array13[k] = array12[k]);
                    }
                }
                else
                {
                    int num5 = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
                    int num6 = image.Stride - imageWidth * num5;
                    for (int l = 0; l < imageHeight; l++)
                    {
                        int num7 = 0;
                        while (num7 < imageWidth)
                        {
                            int num4 = objectLabels[num];
                            if (num4 != 0)
                            {
                                if (num7 < array[num4])
                                {
                                    array[num4] = num7;
                                }

                                if (num7 > array3[num4])
                                {
                                    array3[num4] = num7;
                                }

                                if (l < array2[num4])
                                {
                                    array2[num4] = l;
                                }

                                if (l > array4[num4])
                                {
                                    array4[num4] = l;
                                }

                                array5[num4]++;
                                array6[num4] += num7;
                                array7[num4] += l;
                                byte b2 = ptr[2];
                                byte b3 = ptr[1];
                                byte b4 = *ptr;
                                array8[num4] += b2;
                                array9[num4] += b3;
                                array10[num4] += b4;
                                array11[num4] += b2 * b2;
                                array12[num4] += b3 * b3;
                                array13[num4] += b4 * b4;
                            }

                            num7++;
                            num++;
                            ptr += num5;
                        }

                        ptr += num6;
                    }
                }

                blobs.Clear();
                for (int m = 1; m <= objectsCount; m++)
                {
                    int num8 = array5[m];
                    Blob blob = new Blob(m, new Rectangle(array[m], array2[m], array3[m] - array[m] + 1, array4[m] - array2[m] + 1));
                    blob.Area = num8;
                    blob.Fullness = (double)num8 / (double)((array3[m] - array[m] + 1) * (array4[m] - array2[m] + 1));
                    blob.CenterOfGravity = new Point((float)array6[m] / (float)num8, (float)array7[m] / (float)num8);
                    blob.ColorMean = Color.FromArgb((byte)(array8[m] / num8), (byte)(array9[m] / num8), (byte)(array10[m] / num8));
                    blob.ColorStdDev = Color.FromArgb((byte)System.Math.Sqrt(array11[m] / num8 - blob.ColorMean.R * blob.ColorMean.R), (byte)System.Math.Sqrt(array12[m] / num8 - blob.ColorMean.G * blob.ColorMean.G), (byte)System.Math.Sqrt(array13[m] / num8 - blob.ColorMean.B * blob.ColorMean.B));
                    blobs.Add(blob);
                }
            }
        }
    }
}
