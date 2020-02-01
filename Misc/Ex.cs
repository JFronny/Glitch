using System.Reflection;
using ImageProcessor;

namespace Misc
{
    public static class Ex
    {
        public static ImageFactory InvertColor(this ImageFactory factory)
        {
            if (factory.ShouldProcess)
            {
                ColorInverter processor = new ColorInverter();
                factory.CurrentImageFormat.ApplyProcessor(processor.ProcessImage, factory);
            }
            return factory;
        }
    }
}