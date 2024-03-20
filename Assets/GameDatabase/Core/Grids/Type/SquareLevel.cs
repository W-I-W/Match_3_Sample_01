using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Bow.Data
{
    [CreateAssetMenu(fileName = "Square", menuName = "Database/TypeGrid/Square", order = 70)]
    public class SquareLevel : GridBase<Chip>
    {
        private readonly float HalfUnit = 0.5f;

        public override Map<Chip> Generate()
        {
            Map<Chip> map = new Map<Chip>(m_Size);
            float halfX = map.size.x / 2f - HalfUnit;
            float halfY = map.size.y / 2f - HalfUnit;

            for (int x = 0; x < m_Size.x; x++)
            {
                for (int y = 0; y < m_Size.y; y++)
                {
                    map.cells[x, y].position = new Vector2(-halfX + x, -halfY + y);
                    map.cells[x, y].border = false;
                }
            }
            return map;
        }
    }
}
