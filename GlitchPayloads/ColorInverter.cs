using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Common.Exceptions;
using ImageProcessor.Imaging;
using ImageProcessor.Processors;

namespace GlitchPayloads
{
    public class ColorInverter : IGraphicsProcessor
    {
        public Image ProcessImage(ImageFactory factory)
        {
            Bitmap? newImage = null;
            Image image = factory.Image;
            try
            {
                newImage = new Bitmap(image);
                newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                int width = image.Width;
                int height = image.Height;

                using (FastBitmap fastBitmap = new FastBitmap(newImage))
                {
                    Parallel.For(0, height, y =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color currentColor = fastBitmap.GetPixel(x, y);
                            byte r = currentColor.R;
                            byte g = currentColor.G;
                            byte b = currentColor.B;
                            byte a = currentColor.A;
                            fastBitmap.SetPixel(x, y, Color.FromArgb(a, 255 - r, 255 - g, 255 - b));
                        }
                    });
                }

                image.Dispose();
                image = newImage;
            }
            catch (Exception ex)
            {
                newImage?.Dispose();

                throw new ImageProcessingException("Error processing image with " + GetType().Name, ex);
            }

            return image;
        }

        public dynamic? DynamicParameter { get; set; }
        public Dictionary<string, string>? Settings { get; set; }
    }
}