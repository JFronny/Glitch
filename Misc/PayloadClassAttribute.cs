using System;

namespace Misc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class PayloadClassAttribute : Attribute
    {
    }
}