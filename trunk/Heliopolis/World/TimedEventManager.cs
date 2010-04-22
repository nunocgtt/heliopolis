using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World
{
    public class TimedEventManager : GameWorldObject
    {
        private TimeSpan totalGameTime;
        private SortedDictionary<TimeSpan, TimedEventor> eventorsByTime = new SortedDictionary<TimeSpan, TimedEventor>();
        private Dictionary<TimedEventor, TimeSpan> listOfActiveEventors = new Dictionary<TimedEventor, TimeSpan>();

        public int Scale { get; set; }
        public bool Paused { get; set; }
        public TimeSpan CurrentProcessingTimeSpan { get; set; }

        /// <summary>
        /// Initialises a new instance of the ActorManager class.
        /// </summary>
        /// <param name="_owner">The owning game world.</param>
        public TimedEventManager(GameWorld _owner)
            : base(_owner)
        {
            Scale = 1;
            Paused = true;
            CurrentProcessingTimeSpan = new TimeSpan(0);
        }

        /// <summary>
        /// Main game tick processing.
        /// </summary>
        /// <param name="timeSpan">The time since the last tick.</param>
        public void Tick(TimeSpan timeSpan)
        {
            if (Paused)
                return;

            // Implement scaling here
            totalGameTime = TimeSpan.FromTicks(totalGameTime.Add(timeSpan).Ticks * Scale);

            if (eventorsByTime.Count > 0)
            {
                /* We want to process all the actors whos next absolute action time
                   is less than the current game time tick */

                KeyValuePair<TimeSpan, TimedEventor> kvp = eventorsByTime.First();
                while (kvp.Key.CompareTo(totalGameTime) > 0)
                {
                    CurrentProcessingTimeSpan = kvp.Key;
                    kvp.Value.Tick(totalGameTime);
                    if (!kvp.Value.Disabled)
                    {
                        eventorsByTime.Remove(kvp.Key);
                        // need to be careful about adding the same key so vary by 100 nanoseconds
                        while (eventorsByTime.ContainsKey(kvp.Value.NextAbsoluteActionTime))
                        {
                            kvp.Value.IncrementActionTime(new TimeSpan(1));
                        }
                        eventorsByTime.Add(kvp.Value.NextAbsoluteActionTime, kvp.Value);
                        listOfActiveEventors[kvp.Value] = kvp.Value.NextAbsoluteActionTime;
                    }
                    if (eventorsByTime.Count > 0)
                        kvp = eventorsByTime.First();
                    else
                        break;
                }
            }
        }

        public void StartTimedEventor(TimedEventor timedEventor, TimeSpan firstActionTime)
        {
            while (eventorsByTime.ContainsKey(timedEventor.NextAbsoluteActionTime))
            {
                timedEventor.IncrementActionTime(new TimeSpan(1));
            }
            listOfActiveEventors.Add(timedEventor, firstActionTime);
            eventorsByTime.Add(firstActionTime, timedEventor);
            timedEventor.Disabled = false;
        }

        public void StartTimedAtCurrentTime(TimedEventor timedEventor)
        {
            StartTimedEventor(timedEventor, this.CurrentProcessingTimeSpan);
        }

        public void StopTimedEventor(TimedEventor timedEventor)
        {
            TimeSpan timeToRemove = listOfActiveEventors[timedEventor];
            listOfActiveEventors.Remove(timedEventor);
            eventorsByTime.Remove(timeToRemove);
            timedEventor.Disabled = true;
        }
    }
}
