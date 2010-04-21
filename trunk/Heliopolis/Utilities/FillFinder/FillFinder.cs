using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
    public class FillRequest<T>
    {
        public T start;

        /// <summary>
        /// Constructor for PathfindRequest
        /// </summary>
        /// <param name="_start">Point describing the start position for the path search.</param>
        /// <param name="_end">Point describing the end position for the path search.</param>
        public FillRequest(T _start)
        {
            start = _start;
        }

        public override string ToString()
        {
            return " { Start - " + start.ToString();
        }
    }

    public class FillfindAnswer<T>
    {
        public T[] pointsFilled;
        public FillfindAnswer()
        {
        }
    }

    public class FillFinder<T>
    {
        private ISearchAble<T> searchGrid;
        private SearchState searchState;
        public int stepCount;
        private T startNode;
        private Dictionary<T, int> openList;
        private Dictionary<T, int> closedList;
        private TraceManager<T> traceManager = new TraceManager<T>();

        public FillFinder()
        {
            searchState = SearchState.SEARCH_STATE_NOT_INITIALISED;
            openList = new Dictionary<T, int>();
            closedList = new Dictionary<T, int>();
        }

        public void Initialise(ISearchAble<T> _gameGrid)
        {
            searchGrid = _gameGrid;
        }

        public void NewSearch(FillRequest<T> request)
        {
            if (openList.Count > 0)
                openList.Clear();
            if (closedList.Count > 0)
                closedList.Clear();
            startNode = request.start;
            openList.Add(request.start, 0);
            searchState = SearchState.SEARCH_STATE_SEARCHING;
            traceManager.WriteLine("Initialise - " + request.start.ToString(), "fill");
        }

        public SearchState SearchStep(int numberOfSteps)
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                if (searchState != SearchState.SEARCH_STATE_SEARCHING)
                {
                    return searchState;
                }
                stepCount++;
                foreach (KeyValuePair<T, int> kvp in openList)
                {
                    ProcessNextSearchStep(kvp.Key, kvp.Value);
                    break;
                }
                if (openList.Count == 0)
                {
                    searchState = SearchState.SEARCH_STATE_SUCCEEDED;
                }
            }
            return searchState;
        }

        public FillfindAnswer<T> finalResult()
        {
            FillfindAnswer<T> returnMe = new FillfindAnswer<T>();
            returnMe.pointsFilled = new T[closedList.Count];
            closedList.Keys.CopyTo(returnMe.pointsFilled,0);
            return returnMe;
        }

        public void ProcessNextSearchStep(T p, int depth)
        {
            traceManager.WriteLine("ProcessNextSearchStep", "fill");
            List<T> successors =
                searchGrid.GetSuccessors(p);
            traceManager.WriteLine("Successors of " + p.ToString(), "fill");
            traceManager.DisplayContentsOfList(successors, "fill");
            foreach (T successor in successors)
            {
                bool addMeToOpen = true;
                if (openList.ContainsKey(successor))
                    addMeToOpen = false;
                if (closedList.ContainsKey(successor))
                    addMeToOpen = false;
                if (addMeToOpen)
                    openList.Add(successor, depth + 1);
            }
            openList.Remove(p);
            closedList.Add(p, depth);
        }
    }
}
