using Sirenix.OdinInspector;

using UnityEngine;


namespace Bow
{

    [CreateAssetMenu(fileName = "Tile", menuName = "Database/Tile", order = 60)]
    public class Tile : ScriptableObject
    {
        [PreviewField]
        [SerializeField] private Sprite m_Icon;

        public Sprite icon => m_Icon;

        public Vector2Int slotIntex { get; set; }

        public int id { get; set; }
    }
}