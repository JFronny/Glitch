using System;

namespace GlitchPayloads
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PayloadAttribute : Attribute
    {
        public int DefaultDelay;
        public bool IsSafe;
        public int RunAfter;

        /// <summary>
        ///     Standard initializer
        /// </summary>
        /// <param name="isSafe">Whether a warning should be displayed</param>
        /// <param name="runAfter">Seconds to start running this payload after</param>
        /// <param name="defaultDelay">Default delay between repeats in milliseconds</param>
        public PayloadAttribute(bool isSafe, int runAfter, int defaultDelay)
        {
            IsSafe = isSafe;
            RunAfter = runAfter;
            DefaultDelay = defaultDelay;
        }
    }
}