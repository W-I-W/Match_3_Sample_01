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
            LevelGenerator.tileEase = m_GenerateTileEase;
            LevelGenerator.Play(m_Level);
        }
    }
}
