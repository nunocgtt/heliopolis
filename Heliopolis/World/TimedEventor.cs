using System;
using System.Collections.Generic;
using System.Text;

namespace Heliopolis.World
{
    /// <summary>
    /// Represents an object in the world that performs an action every x milliseconds. 
    /// </summary>
    /// <remarks>The object ties into the main game loop, and is designed to execute actions
    /// on a time basis. The main game loop controls the timing, and will tell TimedEventors
    /// to perform their actions when necessary.</remarks>
    [Serializable]
    public class TimedEventor : GameWorldObject
    {
        /// <summary>
        /// This delegate is required to be pointing to the action that is to be
        /// performed next.
        /// </summary>
        /// <param name="absoluteMilliseconds">The absolute game time in milliseconds</param>
        public delegate void ProcessTick(TimeSpan absoluteMilliseconds);

        /// <summary>
        /// The time between the last action time, and the next one.
        /// </summary>
        protected TimeSpan nextActionTime;
        /// <summary>
        /// The absolute game time for the next action.
        /// </summary>
        protected TimeSpan nextAbsoluteActionTime;
        /// <summary>
        /// The actual action to be performed.
        /// </summary>
        protected ProcessTick processTick;

        /// <summary>
        /// Initialises a new instance of the TimedEventor class.
        /// </summary>
        /// <param name="_owner">The owning GameWorld.</param>
        public TimedEventor(GameWorld _owner)
            : base(_owner)
        {
            nextAbsoluteActionTime = TimeSpan.FromMilliseconds(0);
        }

        private bool disabled = false;

        public bool Disabled 
        {
            get
            {
                return disabled;
            }
            set
            {
                // If we are enabling this timed eventor, we need to re-add it into the manager
                if (disabled && !value)
                {
                    owner.TimedEventManager.StartTimedAtCurrentTime(this);
                }
                // if we are disabling, remove from the event management
                else if (!disabled && value)
                {
                    owner.TimedEventManager.StopTimedEventor(this);
                }
                disabled = value;
            }
        }

        /// <summary>
        /// Set up the next action time by incrementing it by the provided amount.
        /// </summary>
        /// <remarks>This method gets called after this TimedEventor has processed it's action.</remarks>
        /// <param name="amount">The amount of time till the next action, relative to the action just processed.</param>
        public void IncrementActionTime(TimeSpan amount)
        {
            nextActionTime = nextActionTime.Add(amount);
            nextAbsoluteActionTime = nextAbsoluteActionTime.Add(amount);
        }

        /// <summary>
        /// The absolute game time when the next action is to occur.
        /// </summary>
        public TimeSpan NextAbsoluteActionTime
        {
            get { return nextAbsoluteActionTime; }
        }

        /// <summary>
        /// Set up the next tick to occur in the millseconds provided.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds till the next action.</param>
        protected void SetUpNextTick(TimeSpan milliseconds)
        {
            nextActionTime = milliseconds;
            nextAbsoluteActionTime = nextAbsoluteActionTime.Add(milliseconds);
        }

        /// <summary>
        /// Check to see if this Timed eventor's next action time is up, and perform that action
        /// if it is.
        /// </summary>
        /// <param name="absoluteMilliseconds">The current absolute game time.</param>
        public void Tick(TimeSpan absoluteMilliseconds)
        {
            if (absoluteMilliseconds > nextAbsoluteActionTime)
            {
                processTick(absoluteMilliseconds);
            }
        }
    }
}
