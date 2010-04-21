using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
   
    public enum Direction : int
    {
        North = 0,
        South = 1,
        West = 2,
        East = 3,
        Up = 4,
        Down = 5,
        Nowhere = 6
    }

    public enum SearchState
    {
        SEARCH_STATE_NOT_INITIALISED,
        SEARCH_STATE_SEARCHING,
        SEARCH_STATE_SUCCEEDED,
        SEARCH_STATE_FAILED,
        SEARCH_STATE_OUT_OF_MEMORY,
        SEARCH_STATE_INVALID
    };

    /// <summary>
    /// Class used to solve a path finding problem using the A* algorithm.
    /// </summary>
    public class PathFinder<T>
    {
        private Point max;
        private ISearchAble<T> searchGrid;
	    private SearchState searchState;
        private int stepCount;
        private Node<T> startNode;
        private Node<T> goalNode;
        private bool cancelRequest;
        private Dictionary<T, Node<T>> sortedOpenList;
        private LinkedList<Node<T>> openList;
        private Dictionary<T, Node<T>> closedList;
        private LinkedList<Direction> finalDirections;
        private List<T> possibleSolutions;
        private TraceManager<T> traceManager = new TraceManager<T>();

        public PathFinder()
        {
            createAll();
        }

        public PathFinder(int MaxNodes, ISearchAble<T> _gameGrid, Point maxsize)
        {
            createAll();
            Initialise(MaxNodes, _gameGrid, maxsize);
        }

        private void createAll()
        {
            searchState = SearchState.SEARCH_STATE_NOT_INITIALISED;
            cancelRequest = false;
            openList = new LinkedList<Node<T>>();
            sortedOpenList = new Dictionary<T, Node<T>>();
            closedList = new Dictionary<T, Node<T>>();
            finalDirections = new LinkedList<Direction>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="MaxNodes">Maximum number of nodes searched before the search fails.</param>
        /// <param name="_gameGrid">Contents of the game environment.</param>
        /// <param name="maxsize">X/Y size of the game environment.</param>
        public void Initialise(int MaxNodes, ISearchAble<T> _gameGrid, Point maxsize)
        {
            searchGrid = _gameGrid;
            max = maxsize;
            traceManager.WriteLine("Initialise - " + MaxNodes.ToString() + 
                " - " + maxsize.ToString(), "path");
        }

        /// <summary>
        /// Start a new search.
        /// </summary>
        /// <param name="request">Contains all the necessary information to perform the search by.</param>
        public void NewSearch(PathfindRequest<T> request)
        {
            if (sortedOpenList.Count > 0)
                sortedOpenList.Clear();
            if (openList.Count > 0)
                openList.Clear();
            if (closedList.Count > 0)
                closedList.Clear();
            if (finalDirections.Count > 0)
                finalDirections = new LinkedList<Direction>();
            startNode = new Node<T>(request.start);
            goalNode = new Node<T>(request.end);
            startNode.H = GoalDistanceEstimate(request.start, request.end);
            startNode.F = startNode.H + startNode.G;
            // Add the start node into the open list to begin the search
            pushHeap(startNode);
            stepCount = 0;
            searchState = SearchState.SEARCH_STATE_SEARCHING;
            traceManager.WriteLine("New Request - " + request.ToString(), "path");
            possibleSolutions = request.possibleSolutions;
        }

        /// <summary>
        /// Progress the search 'numberOfSteps' steps.
        /// </summary>
        /// <param name="numberOfSteps">The number of steps performed. If numberOfSteps > the number required for a solution the method will return properly.</param>
        /// <returns>Returns the state of the search after 'numberOfSteps' steps are performed.</returns>
        public SearchState SearchStep(int numberOfSteps)
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                if (searchState != SearchState.SEARCH_STATE_SEARCHING)
                {
                    return searchState;
                }
                if ((openList.Count == 0) || cancelRequest)
                {
                    searchState = SearchState.SEARCH_STATE_FAILED;
                    return searchState;
                }
                stepCount++;
                Node<T> nextNode = popHeap();
                bool hitSolution = false;
                if (nextNode.Position.Equals(goalNode.Position))
                {
                    hitSolution = true;
                }
                else if (possibleSolutions != null)
                {
                    foreach (T p in possibleSolutions)
                    {
                        if (nextNode.Position.Equals(p))
                        {
                            hitSolution = true;
                        }
                    }
                }
                if (hitSolution)
                    FinaliseNodeDetails(nextNode);
                else
                    ProcessNextSearchStep(nextNode);
            }
            return searchState;
        }

        /// <summary>
        /// Returns the directions to follow which describes a successful solution.
        /// </summary>
        /// <returns>A linked list of directions.</returns>
        public PathfindAnswer finalResult()
        {
            if (searchState != SearchState.SEARCH_STATE_SUCCEEDED)
            {
                return null;
            }
            else
            {
                PathfindAnswer returnMe = new PathfindAnswer();
                returnMe.directions = finalDirections;
                returnMe.owner = null;
                return returnMe;
            }
        }
        
        /// <summary>
        /// A sorted heap in descending order of 'f' for the nodes.
        /// Poping off the top of the heap gives the lowest value of 'f'.
        /// This collection will contain a list of nodes to solve, ie the open list.
        /// </summary>
        /// <param name="pushNode">The node to add into the collection.</param>
        private void pushHeap(Node<T> pushNode)
        {
            if (openList.Count == 0)
            {
                openList.AddFirst(pushNode);
                sortedOpenList.Add(pushNode.Position, pushNode);
            }
            else
            {
                // Can we add to the very front?
                if (openList.Last.Value.F >= pushNode.F)
                {
                    openList.AddLast(pushNode);
                    sortedOpenList.Add(pushNode.Position, pushNode);
                }
                else
                {
                    // Otherwise find the correct point to insert
                    LinkedListNode<Node<T>> node = openList.First;
                    while (node != null)
                    {
                        if (node.Value.F <= pushNode.F)
                        {
                            openList.AddBefore(node, pushNode);
                            sortedOpenList.Add(pushNode.Position, pushNode);
                            break;
                        }
                        node = node.Next;
                    }
                }
            }
        }

        /// <summary>
        /// Takes the top element off the collection, with the lowest value of 'f'.
        /// Used when getting the next Node to solve.
        /// </summary>
        /// <returns>Returns the top element.</returns>
        private Node<T> popHeap()
        {
            if (openList.Count > 0)
            {
                Node<T> returnMe = openList.Last.Value;
                openList.RemoveLast();
                sortedOpenList.Remove(returnMe.Position);
                return returnMe;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Run when we have picked a node to solve when it's in the same position as the goal node.
        /// This will set up all the node information which can be returned to provide a solution.
        /// </summary>
        /// <param name="nextNode">Next open node in the solution.</param>
        private void FinaliseNodeDetails(Node<T> nextNode)
        {
            traceManager.WriteLine("FinaliseNodeDetails", "path");
            goalNode.Parent = nextNode.Parent;
            goalNode.ComeFrom = nextNode.ComeFrom;
            if (nextNode != startNode)
            {
                Node<T> nodeChild = goalNode;
                Node<T> nodeParent = goalNode.Parent;
                while (nodeChild != startNode)
                {
                    nodeParent.Child = nodeChild;
                    // Set the direction
                    switch (nodeChild.ComeFrom)
                    {
                        case Direction.South:
                            nodeParent.GoTo = Direction.North;
                            break;
                        case Direction.North:
                            nodeParent.GoTo = Direction.South;
                            break;
                        case Direction.West:
                            nodeParent.GoTo = Direction.East;
                            break;
                        case Direction.East:
                            nodeParent.GoTo = Direction.West;
                            break;
                        case Direction.Up:
                            nodeParent.GoTo = Direction.Down;
                            break;
                        case Direction.Down:
                            nodeParent.GoTo = Direction.Up;
                            break;
                        case Direction.Nowhere:
                            nodeParent.GoTo = Direction.Nowhere;
                            break;
                    }
                    finalDirections.AddFirst(nodeParent.GoTo);
                    nodeChild = nodeParent;
                    nodeParent = nodeParent.Parent;
                }
            }
            traceManager.DisplayContentsOfLinkedList(finalDirections, "path");
            searchState = SearchState.SEARCH_STATE_SUCCEEDED;
        }

        /// <summary>
        /// Performs a step in the solution for the next open node.
        /// </summary>
        /// <param name="nextNode">Next open node to solve.</param>
        private void ProcessNextSearchStep(Node<T> nextNode)
        {
            traceManager.WriteLine("ProcessNextSearchStep", "path");
            T parentPoint = nextNode.Parent.Position;
            T pos = nextNode.Position;
            List<Node<T>> successors = searchGrid.GetSuccessorsWithDir(pos, parentPoint);
            traceManager.WriteLine("Successors of " + nextNode.ToString(), "path");
            traceManager.DisplayContentsOfNodeList(successors, "path");
            Node<T> openNode = null;
            Node<T> closedNode = null;
            foreach (Node<T> successor in successors)
            {
                bool foundOpen = false;
                bool foundClosed = false;
                // in this case, the "cost" is added to g
                // but I think I will keep cost even across all squares
                float newg = nextNode.G + searchGrid.getPathingWeight(successor.Position);

                // check to see if this node exists already on the open list
                if (sortedOpenList.ContainsKey(successor.Position))
                {
                    foundOpen = true;
                    openNode = sortedOpenList[successor.Position];
                }
                if (foundOpen)
                    if (openNode.G <= newg)
                    {
                        // ignore this successor because another solution is better/at least as good
                        continue;
                    }
                if (closedList.ContainsKey(successor.Position))
                {
                    foundClosed = true;
                    closedNode = closedList[successor.Position];
                }
                if (foundClosed)
                    if (closedNode.G <= newg)
                    {
                        // ignore this successor because another solution is better/at least as good
                        continue;
                    }
                successor.Parent = nextNode;
                successor.G = newg;
                successor.H = GoalDistanceEstimate(goalNode.Position, successor.Position);
                successor.F = successor.G + successor.H;
                traceManager.WriteLine("Node added to open list - " + successor.ToString(), "path");
                if (foundClosed)
                {
                    closedList.Remove(closedNode.Position);
                }
                if (foundOpen)
                {
                    openList.Remove(openNode);
                    sortedOpenList.Remove(openNode.Position);
                }
                pushHeap(successor);
            }
            // now that all the successors have been deal with add nextNode onto closed list
            closedList.Add(nextNode.Position,nextNode);
        }

        public virtual float GoalDistanceEstimate(object position, object goal)
        {
            throw new NotImplementedException();
        }
    }
}
