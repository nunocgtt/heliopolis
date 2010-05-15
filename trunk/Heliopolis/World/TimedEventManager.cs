using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Heliopolis.World
{
    public class TimedEventManager : GameWorldObject
    {
        private TimeSpan _totalGameTime;
        private readonly SortedDictionary<TimeSpan, TimedEventor> _eventorsByTime = new SortedDictionary<TimeSpan, TimedEventor>();
        private readonly Dictionary<TimedEventor, TimeSpan> _listOfActiveEventors = new Dictionary<TimedEventor, TimeSpan>();

        public int Scale { get; set; }
        public bool Paused { get; set; }
        public TimeSpan CurrentProcessingTimeSpan { get; set; }

        /// <summary>
        /// Initialises a new instance of the ActorManager class.
        /// </summary>
        /// <param name="owner">The owning game world.</param>
        public TimedEventManager(GameWorld owner)
            : base(owner)
        {
            Scale = 1;
            Paused = true;
            CurrentProcessingTimeSpan = new TimeSpan(0);
        }

        /// <summary>
        /// Main game tick processing.
        /// </summary>
        /// <param name="gameTime">The time since the last tick.</param>
        public void Tick(GameTime gameTime)
        {
            if (Paused)
                return;

            // Implement scaling here
            _totalGameTime = TimeSpan.FromTicks(_totalGameTime.Add(gameTime.ElapsedGameTime).Ticks * Scale);

            if (_eventorsByTime.Count <= 0) return;
            /* We want to process all the actors whos next absolute action time
                   is less than the current game time tick */

            KeyValuePair<TimeSpan, TimedEventor> kvp = _eventorsByTime.First();
            while (kvp.Key.CompareTo(_totalGameTime) <= 0)
            {
                CurrentProcessingTimeSpan = kvp.Key;
                kvp.Value.Tick(_totalGameTime);
                if (!kvp.Value.TimedEventDisabled)
                {
                    _eventorsByTime.Remove(kvp.Key);
                    // need to be careful about adding the same key so vary by 100 nanoseconds
                    while (_eventorsByTime.ContainsKey(kvp.Value.NextAbsoluteActionTime))
                    {
                        kvp.Value.IncrementActionTime(new TimeSpan(1));
                    }
                    _eventorsByTime.Add(kvp.Value.NextAbsoluteActionTime, kvp.Value);
                    _listOfActiveEventors[kvp.Value] = kvp.Value.NextAbsoluteActionTime;
                }
                if (_eventorsByTime.Count > 0)
                    kvp = _eventorsByTime.First();
                else
                    break;
            }
        }

        private void StartTimedEventor(TimedEventor timedEventor, TimeSpan firstActionTime)
        {
            while (_eventorsByTime.ContainsKey(timedEventor.NextAbsoluteActionTime))
            {
                timedEventor.IncrementActionTime(new TimeSpan(1));
            }
            _listOfActiveEventors.Add(timedEventor, firstActionTime);
            _eventorsByTime.Add(firstActionTime, timedEventor);
        }

        public void StartTimedAtCurrentTime(TimedEventor timedEventor)
        {
            StartTimedEventor(timedEventor, this.CurrentProcessingTimeSpan);
        }

        public void StopTimedEventor(TimedEventor timedEventor)
        {
            TimeSpan timeToRemove = _listOfActiveEventors[timedEventor];
            _listOfActiveEventors.Remove(timedEventor);
            _eventorsByTime.Remove(timeToRemove);
        }
    }
}
