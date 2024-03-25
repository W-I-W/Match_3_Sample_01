using Bow.Data;

using DG.Tweening;

using UnityEngine;

namespace Bow
{

    public class LevelEvent : MonoBehaviour
    {
        [SerializeField] private LevelData m_Level;
        [SerializeField] private Ease m_GenerateTileEase;

        public void PlayLevel()
        {
            LevelGenerator generator = new LevelGenerator(m_Level);
            generator.tileEase = m_GenerateTileEase;
            generator.Play();
        }
    }
}
