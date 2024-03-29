using Bow;

using DG.Tweening;

using System.Net;

using UnityEngine;

public class Chip : MonoBehaviour
{

    private Vector3 m_Position = Vector2.zero;

    public new SpriteRenderer renderer { get; set; }

    public new BoxCollider2D collider { get; set; }

    private Vector3 position { get => m_Position; set => m_Position = new Vector3(value.x, value.y, transform.position.z); }

    public Vector2Int matrix { get; set; }

    public int id { get; set; }

    public void SetSlot(TileSlot chip, float duration = 0.2f)
    {
        //float dis = Vector2.Distance(position, chip.position);
        position = chip.position;
        matrix = chip.matrix;
        //return transform.DOMove(position, duration * dis);
    }

    public Tween DoMove(float duration = 0.2f)
    {
        //float dis = Vector2.Distance(position, transform.position)*duration;
        return transform.DOMove(position, duration);
    }

    public void Init(Vector3 startPos)
    {
        transform.position = startPos;
        position = startPos;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        this.position = position;
    }
}
