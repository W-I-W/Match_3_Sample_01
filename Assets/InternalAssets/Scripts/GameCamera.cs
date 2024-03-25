using Bow.Data;

using System;

using UnityEngine;

namespace Bow
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private float m_Margin = 1;

        private readonly float m_Min = 1.6f;

        private Camera m_Camera;

        private void OnValidate()
        {
            m_Camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            LevelManager.instance.onGenerate += Resize;
        }

        private void OnDisable()
        {
            LevelManager.instance.onGenerate -= Resize;
        }

        private void Resize(Vector2 size)
        {
            ResizeWidth(size.x);
        }

        private void ResizeWidth(float w)
        {
            float aspect = (float)Screen.height / (float)Screen.width;
            aspect = Mathf.Max(aspect, m_Min);
            float aspectRound = (float)Math.Round(aspect, 2);
            float height = aspectRound * w / 2;
            height += m_Margin;
            m_Camera.orthographicSize = height;
        }
    }
}
