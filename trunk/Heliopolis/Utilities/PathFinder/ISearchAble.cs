using System.Collections.Generic;

namespace Heliopolis.Utilities.PathFinder
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
        float GetPathingWeight(T point);

        List<Node<T>> GetSuccessorsWithDirection(T point);
        List<Node<T>> GetSuccessorsWithDirectionMinusParent(T point, T parentPoint);
        /// <summary>
        /// Returns a list of nodes accessable from a paricular point. Use in fill algorithms.
        /// </summary>
        /// <param name="point">The point to access from.</param>
        /// <returns>A List of Point</returns>
        List<T> GetSuccessors(T point);
    }
}
