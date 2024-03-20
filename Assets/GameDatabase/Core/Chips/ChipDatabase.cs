
using System.Collections.Generic;

using UnityEngine;

namespace Bow.Data
{
    [CreateAssetMenu(fileName = "Chips", menuName = "Database/Chips", order = 60)]
    public class ChipDatabase : ScriptableObject
    {
        [SerializeField] private List<ChipData> m_Chips;
        [SerializeField] private int m_Depth;

        private Transform m_Parent = null;

        public Transform parent
        {
            get
            {
                if (m_Parent == null)
                {
                    m_Parent = new GameObject(name).transform;
                    m_Parent.position = new Vector3(0, 0, m_Depth);
                }
                return m_Parent;
            }
            set => m_Parent = value;
        }

        public void RemoveParent()
        {
            Destroy(parent.gameObject);
            parent = null;
        }

        public Chip GetNewChip(Vector2 seed, Vector2 position = default)
        {
            if (m_Chips.Count == 0) return null;

            int index = Mathf.RoundToInt(Mathf.PerlinNoise(seed.x, seed.y) * (m_Chips.Count - 1));

            //Random.Range(0, m_Chips.Count);

            GameObject obj = new GameObject();

            obj.transform.parent = parent.transform;
            obj.transform.localPosition = new Vector3(position.x, position.y, 0);

            AddSprite(obj, index);
            AddCollider(obj, index);
            Chip chip = AddChip(obj);
            chip.id = index;
            return chip;
        }


        private Chip AddChip(GameObject obj)
        {
            Chip chip = obj.AddComponent<Chip>();
            return chip;
        }

        private SpriteRenderer AddSprite(GameObject obj, int index)
        {
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.name = m_Chips[index].Sprite.name;
            renderer.sprite = m_Chips[index].Sprite;
            return renderer;
        }

        private BoxCollider2D AddCollider(GameObject obj, int index)
        {
            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            collider.size -= new Vector2(0.02f, 0.02f);
            return collider;
        }


    }


    [System.Serializable]
    public struct ChipData
    {
        public Sprite Sprite;
    }
}

