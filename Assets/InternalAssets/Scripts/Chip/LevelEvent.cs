using Bow.Data;


using UnityEngine;

namespace Bow
{

    public class LevelEvent : MonoBehaviour
    {
        [SerializeField] private LevelData m_Level;

        public void PlayLevel()
        {
            LevelManager.instance.SelectLevel(m_Level);
        }
    }
}
