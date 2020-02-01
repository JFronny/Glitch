using System;

namespace Misc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class PayloadAttribute : Attribute
    {
        public int DefaultDelay;
        public bool IsSafe;
        public int RunAfter;
        public bool SelfHosted;
        public string Name;

        /// <summary>
        ///     Standard initializer
        /// </summary>
        /// <param name="isSafe">Whether a warning should be displayed</param>
        /// <param name="runAfter">Seconds to start running this payload after</param>
        /// <param name="defaultDelay">Default delay between repeats in milliseconds</param>
        /// <param name="selfHosted">Whether the payloads hosting is self-contained (aka it is ran once)</param>
        /// <param name="name">Payloads name to display on buttons</param>
        public PayloadAttribute(string name, bool isSafe, int runAfter, int defaultDelay, bool selfHosted = false)
        {
            IsSafe = isSafe;
            RunAfter = runAfter;
            DefaultDelay = defaultDelay;
            SelfHosted = selfHosted;
            Name = name;
        }
    }
}