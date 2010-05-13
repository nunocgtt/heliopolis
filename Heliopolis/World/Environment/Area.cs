using System;
using System.Collections.Generic;
using Heliopolis.GraphicsEngine;

namespace Heliopolis.World.Environment
{
    /// <summary>
    /// Contains information about an areas.
    /// </summary>
    /// <remarks>An Area is a collection of tiles that are all accessable from each other.
    /// The idea behind managing these areas is so that actors can quickly check if they can
    /// get to a tile or not by checking to see if they are in the same area.</remarks>
    [Serializable]
    public class Area
    {
        /// <summary>
        /// The number of tiles in this area.
        /// </summary>
        public int MemberCount;
        /// <summary>
        /// The unique ID of this area.
        /// </summary>
        private readonly int _id;
        /// <summary>
        /// All the tiles in this area.
        /// </summary>
        public readonly List<EnvironmentTile> Members;
        /// <summary>
        /// Initialises a new instance of the Area class.
        /// </summary>
        /// <param name="nextGroupId">The ID of this area.</param>
        public Area(int nextGroupId)
        {
            _id = nextGroupId;
            MemberCount = 0;
            Members = new List<EnvironmentTile>();
        }

        /// <summary>
        /// Merge two areas together because they have joined.
        /// </summary>
        /// <param name="areaA">The first area.</param>
        /// <param name="areaB">The second area.</param>
        /// <returns>The ID that they were merged into.</returns>
        public static int MergeTwoAreas(Area areaA, Area areaB)
        {
            Area copyFrom;
            Area copyTo;
            if (areaA.MemberCount >= areaB.MemberCount)
            {
                copyFrom = areaB;
                copyTo = areaA;
            }
            else
            {
                copyFrom = areaA;
                copyTo = areaB;
            }
            foreach (EnvironmentTile memberTile in copyFrom.Members)
            {
                memberTile.AreaID = copyTo._id;
                copyTo.MemberCount++;
                copyTo.Members.Add(memberTile);
            }
            copyFrom.MemberCount = 0;
            copyFrom.Members.Clear();
            return copyTo._id;
        }
    }
}
