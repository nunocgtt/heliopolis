using System.Collections.Generic;
using Heliopolis.Utilities;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.World.Environment
{
    public class AreaDictionary
    {
        private readonly Dictionary<int, Area> _areaDictionary;
        private readonly Environment _world;

        public AreaDictionary(Environment world)
        {
            _areaDictionary = new Dictionary<int, Area>();
            _world = world;
        }

        /// <summary>
        /// Puts all the unacessable tiles into the group of ID -1.
        /// </summary>
        public void BuildInitialWallGroup()
        {
            if (!_areaDictionary.ContainsKey(-1))
            {
                _areaDictionary.Add(-1, new Area(-1));
            }
            for (int i = 0; i < _world.WorldSize.X; i++)
            {
                for (int j = 0; j < _world.WorldSize.Y; j++)
                {
                    if (!_world[i, j].CanAccess)
                    {
                        _areaDictionary[-1].MemberCount++;
                        _areaDictionary[-1].Members.Add(_world[i, j]);
                    }
                }
            }
        }


        /// <summary>
        /// Builds all the areas.
        /// </summary>
        public void BuildInitialGroups()
        {
            int nextGroupId = 0;
            EnvironmentTile startTile;
            while ((startTile = _world.FindUngroupedTile()) != null)
            {
                nextGroupId++;
                if (!_areaDictionary.ContainsKey(nextGroupId))
                {
                    _areaDictionary.Add(nextGroupId, new Area(nextGroupId));
                }
                FillRequest<Point> newRequest = new FillRequest<Point>(startTile.Position);
                Global.FillFinder.NewSearch(newRequest);
                SearchState returnState = Global.FillFinder.SearchStep(_world.WorldSize.X * _world.WorldSize.Y);
                if (returnState == SearchState.SearchStateSucceeded)
                {
                    FillfindAnswer<Point> theAnswer = Global.FillFinder.FinalResult();
                    foreach (Point p in theAnswer.PointsFilled)
                    {
                        _world[p.X, p.Y].AreaID = nextGroupId;
                        _areaDictionary[nextGroupId].MemberCount++;
                        _areaDictionary[nextGroupId].Members.Add(_world[p.X, p.Y]);
                    }
                }
            }
        }

        /// <summary>
        /// If a tile access flag has changed, this could preclude the joining or separation
        /// of areas. This method will handle that logic of joining/separating.
        /// </summary>
        /// <param name="theTile">The tile changing state.</param>
        public void ManageAccessStateChange(EnvironmentTile theTile)
        {
            if (theTile.CanAccess)
            {
                _areaDictionary[-1].Members.Remove(theTile);
                _areaDictionary[-1].MemberCount--;
                // Here we need to check if two areas have merged
                List<Point> accessPoints = theTile.GetAdjacentAccessPoints();
                foreach (Point point in accessPoints)
                {
                    foreach (Point pointTwo in accessPoints)
                    {
                        if (point != pointTwo)
                        {
                            EnvironmentTile firstTile = _world[point];
                            EnvironmentTile secondTile = _world[pointTwo];
                            if (firstTile.AreaID != secondTile.AreaID)
                            {
                                // Merge requried
                                Area mergeOne = _areaDictionary[firstTile.AreaID];
                                Area mergeTwo = _areaDictionary[secondTile.AreaID];
                                theTile.AreaID = Area.MergeTwoAreas(mergeOne, mergeTwo);
                            }
                        }
                    }
                }
                if (theTile.AreaID == -1)
                {
                    theTile.AreaID = _world[accessPoints[0]].AreaID;
                }
            }
            else
            {
                // now here we need to use the edge state to determine splitting up an area
                _areaDictionary[theTile.AreaID].Members.Remove(theTile);
                _areaDictionary[theTile.AreaID].MemberCount--;
                theTile.AreaID = -1;
            }
            _areaDictionary[theTile.AreaID].Members.Add(theTile);
            _areaDictionary[theTile.AreaID].MemberCount++;
        }
    }
}