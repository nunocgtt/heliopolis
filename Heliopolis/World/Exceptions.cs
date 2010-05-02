using System;

namespace Heliopolis.World
{
    /// <summary>
    /// Custom exception for finding object problems
    /// </summary>
    public class ItemNotFound : Exception
    {
        /// <summary>
        /// Initialises a new instance of the ItemNotFound exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public ItemNotFound(string message)
            : base(message)
        {
        }
    }
}
