
using UnityEngine;

namespace Bow
{
    public struct TileSlot
    {
        public Tile slot;

        public Chip chip { get; set; }

        public Vector2 position { get; set; }

        public Vector2Int matrix { get; set; }

        public bool isSlot { get; set; }

    }
}
