using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.Utilities
{
    /// <summary>
    /// Interface for searching nodes.
    /// </summary>
    public interface ISearchAble<T>
    {
        /// <summary>
        /// How slow something would move over this tile.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        float getPathingWeight(T point);
        /// <summary>
        /// Returns a list of Node accessable from a paricular point.
        /// </summary>
        /// <param name="point">The point to access from.</param>
        /// <param name="parentPoint">The point that this point has already been access from. If there is no parent, pass in null.</param>
        /// <returns>A list of Node</returns>
        List<Node<T>> GetSuccessors(T point, T parentPoint);
        /// <summary>
        /// Returns a list of nodes accessable from a paricular point. Use in fill algorithms.
        /// </summary>
        /// <param name="point">The point to access from.</param>
        /// <returns>A List of Point</returns>
        List<T> GetSuccessors(T point);
    }
}
