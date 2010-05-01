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
    public abstract class TimedEventor : GameWorldObject
    {
        /// <summary>
        /// This delegate is required to be pointing to the action that is to be
        /// performed next.
        /// </summary>
        /// <param name="absoluteMilliseconds">The absolute game time in milliseconds</param>
        public abstract void ExecuteTick(TimeSpan absoluteMilliseconds);

        /// <summary>
        /// The time between the last action time, and the next one.
        /// </summary>
        protected TimeSpan NextActionTime;
        /// <summary>
        /// The absolute game time for the next action.
        /// </summary>
        private TimeSpan _nextAbsoluteActionTime;

        /// <summary>
        /// Initialises a new instance of the TimedEventor class.
        /// </summary>
        /// <param name="owner">The owning GameWorld.</param>
        protected TimedEventor(GameWorld owner)
            : base(owner)
        {
            _nextAbsoluteActionTime = TimeSpan.FromMilliseconds(0);
        }

        private bool _disabled = true;

        public bool TimedEventDisabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                // If we are enabling this timed eventor, we need to re-add it into the manager
                if (_disabled && !value)
                {
                    Owner.TimedEventManager.StartTimedAtCurrentTime(this);
                }
                // if we are disabling, remove from the event management
                else if (!_disabled && value)
                {
                    Owner.TimedEventManager.StopTimedEventor(this);
                }
                _disabled = value;
            }
        }


        /// <summary>
        /// Set up the next action time by incrementing it by the provided amount.
        /// </summary>
        /// <remarks>This method gets called after this TimedEventor has processed it's action.</remarks>
        /// <param name="amount">The amount of time till the next action, relative to the action just processed.</param>
        public void IncrementActionTime(TimeSpan amount)
        {
            NextActionTime = NextActionTime.Add(amount);
            _nextAbsoluteActionTime = _nextAbsoluteActionTime.Add(amount);
        }

        /// <summary>
        /// The absolute game time when the next action is to occur.
        /// </summary>
        public TimeSpan NextAbsoluteActionTime
        {
            get { return _nextAbsoluteActionTime; }
        }

        /// <summary>
        /// Set up the next tick to occur in the millseconds provided.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds till the next action.</param>
        protected void SetUpNextTick(TimeSpan milliseconds)
        {
            NextActionTime = milliseconds;
            _nextAbsoluteActionTime = _nextAbsoluteActionTime.Add(milliseconds);
        }

        /// <summary>
        /// Check to see if this Timed eventor's next action time is up, and perform that action
        /// if it is.
        /// </summary>
        /// <param name="absoluteMilliseconds">The current absolute game time.</param>
        public void Tick(TimeSpan absoluteMilliseconds)
        {
            if (absoluteMilliseconds > _nextAbsoluteActionTime)
            {
                ExecuteTick(absoluteMilliseconds);
            }
        }
    }
}
