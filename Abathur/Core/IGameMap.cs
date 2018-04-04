using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Core {
    public interface IGameMap {
        /// <summary>
        /// Find a valid placement for a barrack/starport/factory with techlab/reactor.
        /// </summary>
        /// <param name="point">Point to find placement near</param>
        /// <returns>Valid placement near given point</returns>
        IPosition FindPlacementWithAddOn(Point2D point);

        /// <summary>
        /// Find valid placement for the given unit type (will detect need for creep and energy)
        /// </summary>
        /// <param name="structure">Structure to find valid placement for</param>
        /// <param name="point">Point to find placement near</param>
        /// <param name="spacing">Optional spacing around structure</param>
        /// <returns>Valid placement near given point</returns>
        IPosition FindPlacement(UnitTypeData structure,Point2D point,int spacing = 0);

        /// <summary>
        /// Check if the given point is a valid place for structure type.
        /// </summary>
        /// <param name="structure">Structure to find validate placement for</param>
        /// <param name="point">Point to validate</param>
        /// <param name="spacing">Optional spacing requirement</param>
        /// <returns></returns>
        bool ValidPlacement(UnitTypeData structure,Point2D point,int spacing = 0);

        /// <summary>
        /// Check for Creep at position.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        bool IsCreep(int x, int y);

        /// <summary>
        /// Check if position is powered by pylon.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        bool IsPowered(int x, int y);

        /// <summary>
        /// Check if position is blocked.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        bool IsBlocked(int x,int y);

        /// <summary>
        /// Reserve a space for structure on the blocked grid (will prevent production manager from building on it)
        /// </summary>
        /// <param name="structure">Structure type to reserve space for</param>
        /// <param name="p">Center of structure</param>
        void Reserve(UnitTypeData structure,Point2D p);
    }
}