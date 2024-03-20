using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace Bow.Data
{

    public abstract class GridBase<T> : ScriptableObject
    {
        [SerializeField] protected Vector2Int m_Size;
        public abstract Map<T> Generate();
    }
}
