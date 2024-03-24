using Bow;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Chip : MonoBehaviour
{
    public Vector2 position { get => transform.position; set => transform.position = new Vector3(value.x, value.y, transform.position.z); }

    public Vector2Int matrix { get; set; }

    public int id { get; set; }


    public Chip SetSlot( TileSlot chip)
    {
        position = chip.position;
        matrix = chip.matrix;
        return this;
    }
}
