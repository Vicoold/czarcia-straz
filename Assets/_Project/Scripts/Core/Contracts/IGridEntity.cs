using UnityEngine;

namespace SlavicTD.Core
{
    /// <summary>
    /// Shared contract for all placeable entities on the 1.5m world grid:
    /// Walls (1x1), Towers (2x2), Cottages (2x2), Barn Barracks (3x2), Sanctuary (4x4).
    /// </summary>
    public interface IGridEntity
    {
        Vector2Int GridOrigin { get; }
        Vector2Int GridSize { get; }
        void OnPlacedOnGrid(Vector2Int origin);
        void OnRemovedFromGrid();
    }
}
