using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Heliopolis.World;

namespace Heliopolis.Utilities
{
    /* Some quick notes to remember about the edge traversal system.
     *  - NodePoints at the moment only exist to provde a way for edges to point to the same
     *    nodes in memory. At the moment, they dont really acheive anything.
     *  - All edges go in one direction, their right side should contain the empty block.
     *  - A lot of the initial calculations can be pre-processed. (TODO)
     *  - At the moment there are two methods run when adding/removing a block.
     *    Later on these can be refactored into one method properly.
     * 
     * 
     * 
     */

    public class NodePoint
    {
        public Point position;
        public bool dead;
        public NodePoint(Point _position, bool _dead)
        {
            position = _position;
            dead = _dead;
        }
        public Edge goingUp = null;
        public Edge goingLeft = null;
        public Edge goingRight = null;
        public Edge goingDown = null;
    }

    public class Edge
    {
        //Note edge always points clockwise, A to B
        public NodePoint A;
        public NodePoint B;
        public EnvironmentTile leftTile = null;
        public EnvironmentTile rightTile = null;
        public int groupId = 0;
        public Edge nextEdge;

        public void Reverse()
        {
            NodePoint swapNode = A;
            A = B;
            B = swapNode;
            EnvironmentTile swapTile = leftTile;
            leftTile = rightTile;
            rightTile = swapTile;
        }
    }

    public class EdgeTraverse
    {
        private Dictionary<Point, NodePoint> nodeList;
        private Dictionary<Point, Edge> edgeList;
        private Dictionary<Point, Edge> edgeListTwo;
        private Environment environment;

        public Point topLeftOffset = new Point(0, 0);
        public Point topRightOffset = new Point(1, 0);
        public Point bottomLeftOffset = new Point(0, 1);
        public Point bottomRightOffset = new Point(1, 1);


        public EdgeTraverse(Environment _gameWorld)
        {
            edgeList = new Dictionary<Point, Edge>();
            edgeListTwo = new Dictionary<Point, Edge>();
            nodeList = new Dictionary<Point, NodePoint>();
            environment = _gameWorld;
        }

        public void buildEdgeData()
        {
            // This will create all the nodes
            BuildNodeDictionary();
            // This will create all the edges
            BuildEdgeList();
        }

        public void BuildNodeDictionary()
        {
            Point worldSize = environment.WorldSize;
            for (int x = 0; x <= worldSize.X; x++)
            {
                for (int y = 0; y <= worldSize.Y; y++)
                {
                    bool upLeftBlockOpen = true;
                    bool upRightBlockOpen = true;
                    bool bottomLeftBlockOpen = true;
                    bool bottomRightBlockOpen = true;
                    if (x == 0)
                        upLeftBlockOpen = false;
                    else if (y == 0)
                        upLeftBlockOpen = false;
                    else
                    {
                        if (!environment.GameWorld[x-1,y-1].CanAccess)
                            upLeftBlockOpen = false;
                    }
                    if (x >= worldSize.X)
                        upRightBlockOpen = false;
                    else if (y == 0)
                        upRightBlockOpen = false;
                    else
                    {
                        if (!environment.GameWorld[x,y-1].CanAccess)
                            upRightBlockOpen = false;
                    }

                    if (x == 0)
                        bottomLeftBlockOpen = false;
                    else if (y >= worldSize.Y)
                        bottomLeftBlockOpen = false;
                    else
                    {
                        if (!environment.GameWorld[x - 1, y].CanAccess)
                            bottomLeftBlockOpen = false;
                    }
                    if (x >= worldSize.X)
                        bottomRightBlockOpen = false;
                    else if (y >= worldSize.Y)
                        bottomRightBlockOpen = false;
                    else
                    {
                        if (!environment.GameWorld[x, y].CanAccess)
                            bottomRightBlockOpen = false;
                    }
                    bool isdead = (upLeftBlockOpen &&
                        upRightBlockOpen &&
                        bottomLeftBlockOpen &&
                        bottomRightBlockOpen);
                    NodePoint addNode = new NodePoint(new Point(x, y), isdead);
                    nodeList.Add(new Point(x, y), addNode);
                }
            }
        }

        public void BuildEdgeList()
        {
            Point worldSize = environment.WorldSize;
            for (int x = 0; x < worldSize.X; x++)
            {
                for (int y = 0; y < worldSize.Y; y++)
                {
                    EnvironmentTile theTile = environment.GameWorld[x,y];
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
            addEdge.A = nodeList[addA];
            addEdge.B = nodeList[addB];
            addEdge.leftTile = leftNormal;
            addEdge.rightTile = rightNormal;
            if (reverse)
            {
                addEdge.Reverse();
            }
            if (edgeList.ContainsKey(addEdge.A.position))
                edgeListTwo.Add(addEdge.A.position, addEdge);
            else
                edgeList.Add(addEdge.A.position, addEdge);
            nodeList[addEdge.A.position].goingRight = addEdge;
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
                offsetPoint(ref pointA, topLeftOffset);
                offsetPoint(ref pointB, topRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.NorthTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                // comes from the top right, this tile on the left normal
                offsetPoint(ref offset, topRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.West))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, bottomLeftOffset);
                offsetPoint(ref pointB, topLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.WestTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                // we want to remove the edge starting at top left
                // with this tile on the left normal
                offsetPoint(ref offset, topLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.East))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, topRightOffset);
                offsetPoint(ref pointB, bottomRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.EastTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, bottomRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, false);
            }
            if (!tileChange.TileCanAccess(Direction.South))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, bottomRightOffset);
                offsetPoint(ref pointB, bottomLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.SouthTile, swapDirections);
            }
            else if (botherRemoving)
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, bottomLeftOffset);
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
                offsetPoint(ref pointA, topLeftOffset);
                offsetPoint(ref pointB, topRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.NorthTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, topLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.West))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, bottomLeftOffset);
                offsetPoint(ref pointB, topLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.WestTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, bottomLeftOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.East))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, topRightOffset);
                offsetPoint(ref pointB, bottomRightOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.EastTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, topRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
            if (tileChange.TileCanAccess(Direction.South))
            {
                Point pointA = tileChange.Position;
                Point pointB = tileChange.Position;
                offsetPoint(ref pointA, bottomRightOffset);
                offsetPoint(ref pointB, bottomLeftOffset);
                CreateEdge(pointA, pointB, tileChange, tileChange.SouthTile, swapDirections);
            }
            else
            {
                Point offset = tileChange.Position;
                offsetPoint(ref offset, bottomRightOffset);
                RemoveEdgeFromCollections(offset, tileChange, true);
            }
        }

        public void RemoveEdgeFromCollections(Point A, EnvironmentTile normalTile, bool normalFacesRight)
        {
            if (edgeListTwo.ContainsKey(A))
            {
                Edge edgeToRemove = edgeListTwo[A];
                if (normalFacesRight)
                {
                    if ((edgeToRemove.rightTile == normalTile))
                    {
                        edgeListTwo.Remove(A);
                    }
                }
                else 
                {
                    if ((edgeToRemove.leftTile == normalTile))
                    {
                        edgeListTwo.Remove(A);
                    }
                }
            }
            if (edgeList.ContainsKey(A))
            {
                Edge edgeToRemove = edgeList[A];
                if (normalFacesRight)
                {
                    if (edgeToRemove.rightTile == normalTile)
                    {
                        edgeList.Remove(A);
                        if (edgeListTwo.ContainsKey(A))
                        {
                            Edge edgeToTransfer = edgeListTwo[A];
                            edgeListTwo.Remove(A);
                            edgeList.Add(A, edgeToTransfer);
                        }
                    }
                }
                else
                {
                    if (edgeToRemove.leftTile == normalTile)
                    {
                        edgeList.Remove(A);
                        if (edgeListTwo.ContainsKey(A))
                        {
                            Edge edgeToTransfer = edgeListTwo[A];
                            edgeListTwo.Remove(A);
                            edgeList.Add(A, edgeToTransfer);
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
