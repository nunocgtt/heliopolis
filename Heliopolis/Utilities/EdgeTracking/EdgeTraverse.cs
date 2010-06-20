using System.Collections.Generic;
using Heliopolis.World.Environment;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.EdgeTracking
{
    /* Some quick notes to remember about the edge traversal system.
     *  - NodePoints at the moment only exist to provde a way for edges to point to the same
     *    nodes in memory. At the moment, they dont really acheive anything.
     *  - All edges go in one direction, their right side should contain the empty block.
     *  - A lot of the initial calculations can be pre-processed.
     *  - At the moment there are two methods run when adding/removing a block.
     *    Later on these can be refactored into one method properly.
     * 
     * 
     * 
     */

    public class EdgeTraverse
    {
        private readonly Dictionary<Point, NodePoint> _nodeList;
        private readonly Dictionary<Point, Edge> _edgeList;
        private readonly Dictionary<Point, Edge> _edgeListTwo;
        private readonly Environment _environment;

        private readonly Point _topLeftOffset = new Point(0, 0);
        private readonly Point _topRightOffset = new Point(1, 0);
        private readonly Point _bottomLeftOffset = new Point(0, 1);
        private readonly Point _bottomRightOffset = new Point(1, 1);


        public EdgeTraverse(Environment gameWorld)
        {
            _edgeList = new Dictionary<Point, Edge>();
            _edgeListTwo = new Dictionary<Point, Edge>();
            _nodeList = new Dictionary<Point, NodePoint>();
            _environment = gameWorld;
        }

        public void BuildEdgeData()
        {
            // This will create all the nodes
            BuildNodeDictionary();
            // This will create all the edges
            BuildEdgeList();
        }

        private void BuildNodeDictionary()
        {
            Point worldSize = _environment.WorldSize;
            for (int x = 0; x <= worldSize.X; x++)
            {
                for (int y = 0; y <= worldSize.Y; y++)
                {
                    //bool upLeftBlockOpen = true;
                    //bool upRightBlockOpen = true;
                    //bool bottomLeftBlockOpen = true;
                    //bool bottomRightBlockOpen = true;
                    //if (x == 0)
                    //    upLeftBlockOpen = false;
                    //else if (y == 0)
                    //    upLeftBlockOpen = false;
                    //else
                    //{
                    //    if (!_environment.GameWorld[x-1,y-1].CanAccess)
                    //        upLeftBlockOpen = false;
                    //}
                    //if (x >= worldSize.X)
                    //    upRightBlockOpen = false;
                    //else if (y == 0)
                    //    upRightBlockOpen = false;
                    //else
                    //{
                    //    if (!_environment.GameWorld[x,y-1].CanAccess)
                    //        upRightBlockOpen = false;
                    //}

                    //if (x == 0)
                    //    bottomLeftBlockOpen = false;
                    //else if (y >= worldSize.Y)
                    //    bottomLeftBlockOpen = false;
                    //else
                    //{
                    //    if (!_environment.GameWorld[x - 1, y].CanAccess)
                    //        bottomLeftBlockOpen = false;
                    //}
                    //if (x >= worldSize.X)
                    //    bottomRightBlockOpen = false;
                    //else if (y >= worldSize.Y)
                    //    bottomRightBlockOpen = false;
                    //else
                    //{
                    //    if (!_environment.GameWorld[x, y].CanAccess)
                    //        bottomRightBlockOpen = false;
                    //}
                    NodePoint addNode = new NodePoint(new Point(x, y));
                    _nodeList.Add(new Point(x, y), addNode);
                }
            }
        }

        private void BuildEdgeList()
        {
            Point worldSize = _environment.WorldSize;
            for (int x = 0; x < worldSize.X; x++)
            {
                for (int y = 0; y < worldSize.Y; y++)
                {
                    EnvironmentTile theTile = _environment.GameWorld[x,y];
                    if (theTile.CanAccess)
                    {
                        CanAccessTileSetupEdges(theTile, false);
                    }
                }
            }
        }

        private void CreateEdge(Point addA, Point addB, EnvironmentTile rightNormal, EnvironmentTile leftNormal, bool reverse)
        {
            Edge addEdge = new Edge();
            addEdge.A = _nodeList[addA];
            addEdge.B = _nodeList[addB];
            addEdge.LeftTile = leftNormal;
            addEdge.RightTile = rightNormal;
            if (reverse)
            {
                addEdge.Reverse();
            }
            if (_edgeList.ContainsKey(addEdge.A.Position))
                _edgeListTwo.Add(addEdge.A.Position, addEdge);
            else
                _edgeList.Add(addEdge.A.Position, addEdge);
            _nodeList[addEdge.A.Position].GoingRight = addEdge;
        }

        private void offsetPoint(ref Point inPoint, Point offset)
        {
            inPoint = new Point(inPoint.X + offset.X, inPoint.Y + offset.Y);
        }

        public void CanAccessTileSetupEdges(EnvironmentTile tileChange, bool botherRemoving)
        {
            bool swapDirections = false;
            if (!tileChange.TileCanAccess(Direction.North))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _topLeftOffset);
                offsetPoint(ref pointB, _topRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.NorthTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                // comes from the top right, this tile on the left normal
                offsetPoint(ref offset, _topRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.West))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _bottomLeftOffset);
                offsetPoint(ref pointB, _topLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.WestTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                // we want to remove the edge starting at top left
                // with this tile on the left normal
                offsetPoint(ref offset, _topLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.East))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _topRightOffset);
                offsetPoint(ref pointB, _bottomRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.EastTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _bottomRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.South))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _bottomRightOffset);
                offsetPoint(ref pointB, _bottomLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.SouthTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _bottomLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
        }

        public void CanNotAccessTileSetupEdges(EnvironmentTile tileChange)
        {
            // in this case, the directions are reversed
            bool swapDirections = true;
            if (tileChange.TileCanAccess(Direction.North))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _topLeftOffset);
                offsetPoint(ref pointB, _topRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.NorthTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _topLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.West))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _bottomLeftOffset);
                offsetPoint(ref pointB, _topLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.WestTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _bottomLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.East))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _topRightOffset);
                offsetPoint(ref pointB, _bottomRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.EastTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _topRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.South))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, _bottomRightOffset);
                offsetPoint(ref pointB, _bottomLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.SouthTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, _bottomRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
        }

        public void RemoveEdgeFromCollections(Point A, EnvironmentTile normalTile, bool normalFacesRight)
        {
            if (_edgeListTwo.ContainsKey(A))
            {
                Edge edgeToRemove = _edgeListTwo[A];
                if (normalFacesRight)
                {
                    if ((edgeToRemove.RightTile == normalTile))
                    {
                        _edgeListTwo.Remove(A);
                    }
                }
                else 
                {
                    if ((edgeToRemove.LeftTile == normalTile))
                    {
                        _edgeListTwo.Remove(A);
                    }
                }
            }
            if (_edgeList.ContainsKey(A))
            {
                Edge edgeToRemove = _edgeList[A];
                if (normalFacesRight)
                {
                    if (edgeToRemove.RightTile == normalTile)
                    {
                        _edgeList.Remove(A);
                        if (_edgeListTwo.ContainsKey(A))
                        {
                            Edge edgeToTransfer = _edgeListTwo[A];
                            _edgeListTwo.Remove(A);
                            _edgeList.Add(A, edgeToTransfer);
                        }
                    }
                }
                else
                {
                    if (edgeToRemove.LeftTile == normalTile)
                    {
                        _edgeList.Remove(A);
                        if (_edgeListTwo.ContainsKey(A))
                        {
                            Edge edgeToTransfer = _edgeListTwo[A];
                            _edgeListTwo.Remove(A);
                            _edgeList.Add(A, edgeToTransfer);
                        }
                    }
                }
            }
        }

        public void CanAccessChanged(EnvironmentTile tileChange)
        {
            if (tileChange.CanAccess)
            {
                CanAccessTileSetupEdges(tileChange, true);
            }
            else
            {
                CanNotAccessTileSetupEdges(tileChange);
            }
        }

        public bool DoEdgesConnect(Edge firstEdge, Edge secondEdge)
        {
            return false;
        }

        // I dont think I will need this
        /*
        public void connectEdges()
        {
            int dictionarySet = 0;
            while (edgeList.Count > 0)
            {
                dictionarySet++;
                Edge oneEdge = getFirstEdge();
                List<Edge> oneSet;
                if (!edgeDictionary.ContainsKey(dictionarySet))
                {
                    oneSet = new List<Edge>();
                    edgeDictionary.Add(dictionarySet, oneSet);
                }
                else
                    oneSet = edgeDictionary[dictionarySet];

                // save the initial edge 
                Point beginPoint = oneEdge.A.position;
                Edge originalEdge = oneEdge;

                // now add it into the oneSet with all its friends
                while (oneEdge.A.position != beginPoint)
                {
                    edgeList.Remove(beginPoint);
                    //oneSet.Add(oneEdge);
                    if (oneEdge.B.position == originalEdge.A.position)
                    {
                        oneEdge.nextEdge = originalEdge;
                    }
                    else
                    {
                        // now need to check for duplicates here
                        if (edgeListTwo.ContainsKey(oneEdge.B.position))
                        {
                            //
                        }
                        else
                        {
                            oneEdge.nextEdge = edgeList[oneEdge.B.position];
                        }
                    }
                    oneEdge = oneEdge.nextEdge;
                }
            }
        }*/

    }
}
