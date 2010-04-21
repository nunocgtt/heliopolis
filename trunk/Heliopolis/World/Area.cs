using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World
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
        public int memberCount;
        /// <summary>
        /// The unique ID of this area.
        /// </summary>
        public int id;
        /// <summary>
        /// All the tiles in this area.
        /// </summary>
        public List<EnvironmentTile> members;
        /// <summary>
        /// Initialises a new instance of the Area class.
        /// </summary>
        /// <param name="_nextGroupId">The ID of this area.</param>
        public Area(int _nextGroupId)
        {
            id = _nextGroupId;
            memberCount = 0;
            members = new List<EnvironmentTile>();
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
            if (areaA.memberCount >= areaB.memberCount)
            {
                copyFrom = areaB;
                copyTo = areaA;
            }
            else
            {
                copyFrom = areaA;
                copyTo = areaB;
            }
            foreach (EnvironmentTile memberTile in copyFrom.members)
            {
                memberTile.AreaID = copyTo.id;
                copyTo.memberCount++;
                copyTo.members.Add(memberTile);
            }
            copyFrom.memberCount = 0;
            copyFrom.members.Clear();
            return copyTo.id;
        }
    }
}
