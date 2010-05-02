using System;
using System.Collections.Generic;
using System.Text;
using Heliopolis.Utilities.PathFinder;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
    public class FillRequest<T>
    {
        public T Start;

        /// <summary>
        /// Constructor for PathfindRequest
        /// </summary>
        /// <param name="start">Point describing the start position for the path search.</param>
        public FillRequest(T start)
        {
            Start = start;
        }

        public override string ToString()
        {
            return " { Start - " + Start.ToString();
        }
    }

    public class FillfindAnswer<T>
    {
        public T[] PointsFilled;
        public FillfindAnswer()
        {
        }
    }

    public class FillFinder<T>
    {
        private ISearchAble<T> _searchGrid;
        private SearchState _searchState;
        public int StepCount;
        private readonly Dictionary<T, int> _openList;
        private readonly Dictionary<T, int> _closedList;
        private readonly TraceManager<T> _traceManager = new TraceManager<T>();

        public FillFinder()
        {
            _searchState = SearchState.SearchStateNotInitialised;
            _openList = new Dictionary<T, int>();
            _closedList = new Dictionary<T, int>();
        }

        public void Initialise(ISearchAble<T> gameGrid)
        {
            _searchGrid = gameGrid;
        }

        public void NewSearch(FillRequest<T> request)
        {
            if (_openList.Count > 0)
                _openList.Clear();
            if (_closedList.Count > 0)
                _closedList.Clear();
            _openList.Add(request.Start, 0);
            _searchState = SearchState.SearchStateSearching;
            _traceManager.WriteLine("Initialise - " + request.Start.ToString(), "fill");
        }

        public SearchState SearchStep(int numberOfSteps)
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                if (_searchState != SearchState.SearchStateSearching)
                {
                    return _searchState;
                }
                StepCount++;
                foreach (KeyValuePair<T, int> kvp in _openList)
                {
                    ProcessNextSearchStep(kvp.Key, kvp.Value);
                    break;
                }
                if (_openList.Count == 0)
                {
                    _searchState = SearchState.SearchStateSucceeded;
                }
            }
            return _searchState;
        }

        public FillfindAnswer<T> FinalResult()
        {
            FillfindAnswer<T> returnMe = new FillfindAnswer<T> {PointsFilled = new T[_closedList.Count]};
            _closedList.Keys.CopyTo(returnMe.PointsFilled,0);
            return returnMe;
        }

        public void ProcessNextSearchStep(T p, int depth)
        {
            _traceManager.WriteLine("ProcessNextSearchStep", "fill");
            List<T> successors =
                _searchGrid.GetSuccessors(p);
            _traceManager.WriteLine("Successors of " + p.ToString(), "fill");
            _traceManager.DisplayContentsOfList(successors, "fill");
            foreach (T successor in successors)
            {
                bool addMeToOpen = true;
                if (_openList.ContainsKey(successor))
                    addMeToOpen = false;
                if (_closedList.ContainsKey(successor))
                    addMeToOpen = false;
                if (addMeToOpen)
                    _openList.Add(successor, depth + 1);
            }
            _openList.Remove(p);
            _closedList.Add(p, depth);
        }
    }
}
