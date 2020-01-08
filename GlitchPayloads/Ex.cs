using System.Reflection;
using ImageProcessor;

namespace GlitchPayloads
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

        public static string GetPayloadName(this MethodInfo payload) => payload.Name.Remove(0, 7);
    }
}