using Sirenix.OdinInspector;

using System;


using UnityEngine;

namespace Bow.Data
{
    [CreateAssetMenu(fileName = "Level", menuName = "Database/Level", order = 50)]
    public class LevelData : SerializedScriptableObject
    {

        [SerializeField, PreviewField] private Sprite m_TileBackground;

        [SerializeField, TabGroup("Chips")] private TileSlot[] m_Chips;

        #region Tiles

        [SerializeField, TabGroup("Tiles")] private TileSlot[,] m_Tiles;
        [SerializeField, TabGroup("Tiles")] private Vector2Int m_Size = new Vector2Int(10, 10);
        [SerializeField, TabGroup("Tiles")] private int m_Seed = 1;

        [Button]
        [TabGroup("Tiles")]
        private void CreateBoard()
        {
            if (m_Chips == null && m_Chips.Length == 0) return;
            m_Tiles = new TileSlot[m_Size.x, m_Size.y];
            int[,] tiles = Match.Create(m_Size, m_Seed, m_Chips.Length);
            m_Tiles = Match.Bundle(m_Chips, tiles);
        }

        #endregion

        public Sprite tileBackground => m_TileBackground;

        public Vector2Int size => m_Size;

        public TileSlot[] chips => m_Chips;

        public int sizeChip => m_Chips.Length;


        public TileSlot[,] slots => m_Tiles;


        private void OnValidate()
        {
            m_Chips = m_Chips ?? new TileSlot[4];

            m_Tiles = m_Tiles ?? new TileSlot[m_Size.x, m_Size.y];

            for (int i = 0; i < m_Chips.Length; i++)
            {
                if (m_Chips[i].slot == null) continue;
                m_Chips[i].slot.id = i;
            }
        }
    }
}
