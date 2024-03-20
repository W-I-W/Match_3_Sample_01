using UnityEngine;
using UnityEngine.Events;

namespace Bow.Data
{
    [CreateAssetMenu(fileName = "Level", menuName = "Database/Level", order = 50)]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private CellData m_BackgroundCell;
        [SerializeField] private ChipDatabase m_Chips;
        [SerializeField] private GridBase<Chip> m_TypeGrid;
        [SerializeField] private int m_Seed = 1;
        [SerializeField] private int m_Scale = 100;
        [SerializeField] private int m_Mod = 100;


        public Vector2Int size { get; private set; }

        public Map<Chip> map { get; private set; }


        public void Play()
        {
            map = m_TypeGrid.Generate();
            SetGridBackground(map);
            SetChips(map);
            size = map.size;
        }

        public void SetGridBackground(Map<Chip> map)
        {
            for (int x = 0; x < map.size.x; x++)
            {
                for (int y = 0; y < map.size.y; y++)
                {
                    if (map.cells[x, y].border) continue;

                    SpriteRenderer renderer = m_BackgroundCell.GetNewSprite();
                    renderer.transform.position = map.cells[x, y].position;
                }
            }
        }

        public void SetChips(Map<Chip> map)
        {
            m_Chips.RemoveParent();

            for (int x = 0; x < map.size.x; x++)
            {
                for (int y = 0; y < map.size.y; y++)
                {
                    GetNewChip(new Vector2Int(x, y));
                }
            }
        }

        public Chip GetNewChip(Vector2Int index)
        {
            m_Seed = m_Seed <= 0 ? 1 : m_Seed;
            Vector2 position = map.cells[index.x, index.y].position;
            Chip renderer = m_Chips.GetNewChip((Vector2)index / m_Mod * m_Scale, position);

            renderer.index = new Vector2Int(index.x, index.y);
            map.cells[index.x, index.y].renderer = renderer;
            return renderer;
        }
    }
}
