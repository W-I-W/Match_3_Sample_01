using UnityEngine;


namespace Bow.Data
{
    public class Map<T>
    {
        public Map(Vector2Int size)
        {
            cells = new Cell<T>[size.x, size.y];
        }

        public Cell<T>[,] cells { get; set; }

        public Vector2Int size
        {
            get => new Vector2Int(cells.GetLength(0), cells.GetLength(1));
        }

    }

    public struct Cell<T>
    {
        public T renderer { get; set; }
        public Vector2 position { get; set; }

        public bool border { get; set; }
    }
}
