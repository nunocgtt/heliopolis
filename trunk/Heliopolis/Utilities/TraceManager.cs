#define TRACE

using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Heliopolis.Utilities
{
    public class TraceManager<T>
    {
        private Dictionary<string, bool> traceOn;

        public TraceManager()
        {
            traceOn = new Dictionary<string,bool>();
            traceOn.Add("path", false);
            traceOn.Add("fill", false);
        }

        public void WriteLine(string line, string category)
        {
#if TRACE
            if (traceOn[category])
            {
                Trace.WriteLine(line);
            }
#endif
        }

        public void DisplayContentsOfLinkedList(LinkedList<Direction> myEnum, string category)
        {
#if TRACE
            if (traceOn[category])
            {
                StringBuilder buildMe = new StringBuilder();
                foreach (Direction o in myEnum)
                {
                    buildMe.Append(o.ToString() + ",");
                }
                Trace.WriteLine(buildMe);
            }
#endif
        }

        public void DisplayContentsOfNodeList(List<Node<T>> myEnum, string category)
        {
#if TRACE
            if (traceOn[category])
            {
                StringBuilder buildMe = new StringBuilder();
                foreach (Node<T> o in myEnum)
                {
                    buildMe.Append(o.ToString() + ",");
                }
                Trace.WriteLine(buildMe);
            }
#endif
        }
        public void DisplayContentsOfList(List<T> myEnum, string category)
        {
#if TRACE
            if (traceOn[category])
            {
                StringBuilder buildMe = new StringBuilder();
                foreach (T o in myEnum)
                {
                    buildMe.Append(o.ToString() + ",");
                }
                Trace.WriteLine(buildMe);
            }
#endif
        }

    }
}
