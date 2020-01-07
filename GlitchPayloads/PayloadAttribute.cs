using System;

namespace GlitchPayloads
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PayloadAttribute : Attribute
    {
    }
}