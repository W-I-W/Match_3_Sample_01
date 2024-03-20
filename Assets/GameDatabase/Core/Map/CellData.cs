using UnityEngine;

namespace Bow.Data
{


    [CreateAssetMenu(fileName = "Map", menuName = "Database/Map", order = 60)]
    public class CellData : ScriptableObject
    {
        [SerializeField] private Sprite m_Cell;
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
        }

        public SpriteRenderer GetNewSprite()
        {
            SpriteRenderer renderer = new GameObject("Cell")
                .AddComponent<SpriteRenderer>();
            renderer.transform.parent = parent.transform;
            renderer.sprite = m_Cell;
            return renderer;
        }
    }
}
