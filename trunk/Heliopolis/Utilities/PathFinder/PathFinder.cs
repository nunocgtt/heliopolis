using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities.PathFinder
{
    public enum Direction
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
        SearchStateNotInitialised,
        SearchStateSearching,
        SearchStateSucceeded,
        SearchStateFailed,
        SearchStateOutOfMemory,
        SearchStateInvalid
    };

    /// <summary>
    /// Class used to solve a path finding problem using the A* algorithm.
    /// </summary>
    public abstract class PathFinder<T>
    {
        private ISearchAble<T> _searchGrid;
	    private SearchState _searchState;
        private int _stepCount;
        private Node<T> _startNode;
        private Node<T> _goalNode;
        private bool _cancelRequest;
        private Dictionary<T, Node<T>> _sortedOpenList;
        private LinkedList<Node<T>> _openList;
        private Dictionary<T, Node<T>> _closedList;
        private LinkedList<Direction> _finalDirections;
        private List<T> _possibleSolutions;
        private bool _singlePointSolution;
        private readonly TraceManager<T> _traceManager = new TraceManager<T>();

        protected PathFinder()
        {
            CreateAll();
        }

        protected PathFinder(int maxNodes, ISearchAble<T> gameGrid, Point maxsize)
        {
            CreateAll();
            Initialise(maxNodes, gameGrid, maxsize);
        }

        private void CreateAll()
        {
            _searchState = SearchState.SearchStateNotInitialised;
            _cancelRequest = false;
            _openList = new LinkedList<Node<T>>();
            _sortedOpenList = new Dictionary<T, Node<T>>();
            _closedList = new Dictionary<T, Node<T>>();
            _finalDirections = new LinkedList<Direction>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxNodes">Maximum number of nodes searched before the search fails.</param>
        /// <param name="gameGrid">Contents of the game environment.</param>
        /// <param name="maxsize">X/Y size of the game environment.</param>
        public void Initialise(int maxNodes, ISearchAble<T> gameGrid, Point maxsize)
        {
            _searchGrid = gameGrid;
            _traceManager.WriteLine("Initialise - " + maxNodes + 
                " - " + maxsize, "path");
        }

        /// <summary>
        /// Start a new search.
        /// </summary>
        /// <param name="request">Contains all the necessary information to perform the search by.</param>
        public void NewSearch(PathfindRequest<T> request)
        {
            if (_sortedOpenList.Count > 0)
                _sortedOpenList.Clear();
            if (_openList.Count > 0)
                _openList.Clear();
            if (_closedList.Count > 0)
                _closedList.Clear();
            if (_finalDirections.Count > 0)
                _finalDirections = new LinkedList<Direction>();
            _startNode = new Node<T>(request.start);
            _singlePointSolution = request.SinglePointSolution;
            if (_singlePointSolution)
                _goalNode = new Node<T>(request.end);
            else
                _goalNode = new Node<T>(request.possibleSolutions[0]);
            _startNode.H = GoalDistanceEstimate(request.start, request.end);
            _startNode.F = _startNode.H + _startNode.G;
            // Add the start node into the open list to begin the search
            PushHeap(_startNode);
            _stepCount = 0;
            _searchState = SearchState.SearchStateSearching;
            _traceManager.WriteLine("New Request - " + request, "path");
            _possibleSolutions = request.possibleSolutions;
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
                if (_searchState != SearchState.SearchStateSearching)
                {
                    return _searchState;
                }
                if ((_openList.Count == 0) || _cancelRequest)
                {
                    _searchState = SearchState.SearchStateFailed;
                    return _searchState;
                }
                _stepCount++;
                Node<T> nextNode = PopHeap();
                bool hitSolution = false;
                if (nextNode.Position.Equals(_goalNode.Position))
                {
                    hitSolution = true;
                }
                else if (!_singlePointSolution)
                {
                    foreach (T p in _possibleSolutions)
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
            return _searchState;
        }

        /// <summary>
        /// Returns the directions to follow which describes a successful solution.
        /// </summary>
        /// <returns>A linked list of directions.</returns>
        public PathfindAnswer FinalResult()
        {
            if (_searchState != SearchState.SearchStateSucceeded)
            {
                return null;
            }
            else
            {
                PathfindAnswer returnMe = new PathfindAnswer {Directions = _finalDirections, Owner = null};
                return returnMe;
            }
        }
        
        /// <summary>
        /// A sorted heap in descending order of 'f' for the nodes.
        /// Poping off the top of the heap gives the lowest value of 'f'.
        /// This collection will contain a list of nodes to solve, ie the open list.
        /// </summary>
        /// <param name="pushNode">The node to add into the collection.</param>
        private void PushHeap(Node<T> pushNode)
        {
            if (_openList.Count == 0)
            {
                _openList.AddFirst(pushNode);
                _sortedOpenList.Add(pushNode.Position, pushNode);
            }
            else
            {
                // Can we add to the very front?
                if (_openList.Last.Value.F >= pushNode.F)
                {
                    _openList.AddLast(pushNode);
                    _sortedOpenList.Add(pushNode.Position, pushNode);
                }
                else
                {
                    // Otherwise find the correct point to insert
                    LinkedListNode<Node<T>> node = _openList.First;
                    while (node != null)
                    {
                        if (node.Value.F <= pushNode.F)
                        {
                            _openList.AddBefore(node, pushNode);
                            _sortedOpenList.Add(pushNode.Position, pushNode);
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
        private Node<T> PopHeap()
        {
            if (_openList.Count > 0)
            {
                Node<T> returnMe = _openList.Last.Value;
                _openList.RemoveLast();
                _sortedOpenList.Remove(returnMe.Position);
                return returnMe;
            }
            return null;
        }

        /// <summary>
        /// Run when we have picked a node to solve when it's in the same position as the goal node.
        /// This will set up all the node information which can be returned to provide a solution.
        /// </summary>
        /// <param name="nextNode">Next open node in the solution.</param>
        private void FinaliseNodeDetails(Node<T> nextNode)
        {
            _traceManager.WriteLine("FinaliseNodeDetails", "path");
            _goalNode.Parent = nextNode.Parent;
            _goalNode.ComeFrom = nextNode.ComeFrom;
            if (nextNode != _startNode)
            {
                Node<T> nodeChild = _goalNode;
                Node<T> nodeParent = _goalNode.Parent;
                while (nodeChild != _startNode)
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
                    _finalDirections.AddFirst(nodeParent.GoTo);
                    nodeChild = nodeParent;
                    nodeParent = nodeParent.Parent;
                }
            }
            _traceManager.DisplayContentsOfLinkedList(_finalDirections, "path");
            _searchState = SearchState.SearchStateSucceeded;
        }

        /// <summary>
        /// Performs a step in the solution for the next open node.
        /// </summary>
        /// <param name="nextNode">Next open node to solve.</param>
        private void ProcessNextSearchStep(Node<T> nextNode)
        {
            _traceManager.WriteLine("ProcessNextSearchStep", "path");
            T parentPoint = nextNode.Parent.Position;
            T pos = nextNode.Position;
            List<Node<T>> successors = _searchGrid.GetSuccessorsWithDir(pos, parentPoint);
            _traceManager.WriteLine("Successors of " + nextNode.ToString(), "path");
            _traceManager.DisplayContentsOfNodeList(successors, "path");
            Node<T> openNode = null;
            Node<T> closedNode = null;
            foreach (Node<T> successor in successors)
            {
                bool foundOpen = false;
                bool foundClosed = false;
                // in this case, the "cost" is added to g
                // but I think I will keep cost even across all squares
                float newg = nextNode.G + _searchGrid.GetPathingWeight(successor.Position);

                // check to see if this node exists already on the open list
                if (_sortedOpenList.ContainsKey(successor.Position))
                {
                    foundOpen = true;
                    openNode = _sortedOpenList[successor.Position];
                }
                if (foundOpen)
                    if (openNode.G <= newg)
                    {
                        // ignore this successor because another solution is better/at least as good
                        continue;
                    }
                if (_closedList.ContainsKey(successor.Position))
                {
                    foundClosed = true;
                    closedNode = _closedList[successor.Position];
                }
                if (foundClosed)
                    if (closedNode.G <= newg)
                    {
                        // ignore this successor because another solution is better/at least as good
                        continue;
                    }
                successor.Parent = nextNode;
                successor.G = newg;
                successor.H = GoalDistanceEstimate(_goalNode.Position, successor.Position);
                successor.F = successor.G + successor.H;
                _traceManager.WriteLine("Node added to open list - " + successor.ToString(), "path");
                if (foundClosed)
                {
                    _closedList.Remove(closedNode.Position);
                }
                if (foundOpen)
                {
                    _openList.Remove(openNode);
                    _sortedOpenList.Remove(openNode.Position);
                }
                PushHeap(successor);
            }
            // now that all the successors have been deal with add nextNode onto closed list
            _closedList.Add(nextNode.Position,nextNode);
        }

        public abstract float GoalDistanceEstimate(T position, T goal);
    }
}
