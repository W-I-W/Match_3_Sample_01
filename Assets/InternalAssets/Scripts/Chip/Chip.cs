using Bow;

using DG.Tweening;

using UnityEngine;

public class Chip : MonoBehaviour
{
    private readonly float Duration = 0.2f;

    private Vector3 m_Position = Vector2.zero;

    public new SpriteRenderer renderer { get; set; }

    public new BoxCollider2D collider { get; set; }

    private Vector3 position { get => m_Position; set => m_Position = new Vector3(value.x, value.y, transform.position.z); }

    public Vector2Int matrix { get; set; }

    public int id { get; set; }

    public Tween SetSlot(TileSlot chip)
    {
        float dis = Vector2.Distance(position, chip.position);
        position = chip.position;
        matrix = chip.matrix;
        return transform.DOMove(position, Duration * dis);
    }

    public void Init(Vector3 startPos)
    {
        transform.position = startPos;
        position = startPos;
    }

    public  void ResetChip(Vector2 startPos)
    {
        transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);
        position = startPos;
    }
}
