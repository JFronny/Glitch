using System;
using ImageProcessor;

namespace Glitch
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

        public static string GetPayloadName(this Action payload) => payload.Method.Name.Remove(0, 7);
    }
}