using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace Bow
{
    public class Chip : MonoBehaviour
    {
        public Vector2Int index { get; set; }

        public Vector2 position { get => transform.localPosition; set => transform.localPosition = value; }

        public int id { get; set; }
    }
}