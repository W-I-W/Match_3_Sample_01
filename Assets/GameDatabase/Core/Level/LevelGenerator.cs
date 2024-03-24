using Bow.Data;

using DG.Tweening;

using UnityEngine;

public static class LevelGenerator
{

    private static readonly int DepthBackground = 10;
    private static readonly int DepthTile = -10;
    private static readonly float HalfUnit = 0.5f;

    private static Sequence m_Seq;

    public static float backgroundSpeed { get; set; } = 0.01f;
    public static float tileSpeed { get; set; } = 0.4f;
    public static Ease tileEase { get; set; } = Ease.OutFlash;

    public static Chip[,] chips { get; set; }

    private static int xCount => LevelManager.instance.level.slots.GetLength(0);

    private static int yCount => LevelManager.instance.level.slots.GetLength(1);

    private static float halfX => xCount / 2f - HalfUnit;

    private static float halfY => yCount / 2f - HalfUnit;

    private static Transform parentTile { get; set; }


    public static void Play(LevelData level)
    {
        LevelManager.instance.SelectLevel(level);
        CreateBackground();
        m_Seq.AppendCallback(() => CreateChips());
    }

    public static Chip GetNewChip(Vector2Int index)
    {
        LevelData level = LevelManager.instance.level;

        GameObject obj = CreateObject(parentTile);
        obj.transform.localScale = (new Vector3());
        SpriteRenderer renderer = obj.AddSprite(level.slots[index.x, index.y].Slot.icon);
        BoxCollider2D collider = obj.AddCollider();
        Chip chip = obj.AddChip(new Vector2Int(index.x, index.y));

        m_Seq.Append(obj.transform.DOScale(1, tileSpeed)
            .SetDelay(Random.Range(0f, 0.2f))
            .SetEase(tileEase));

        level.slots[index.x, index.y].chip = chip;
        Vector2 position = level.slots[index.x, index.y].position;
        renderer.transform.position = new Vector3(position.x, position.y, DepthTile);
        return chip;
    }

    private static void CreateBackground()
    {
        LevelData level = LevelManager.instance.level;
        GameObject parent = new GameObject("Background");
        m_Seq = DOTween.Sequence();
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                if (level.slots[x, y].Slot == null)
                {
                    level.slots[x, y].isSlot = false;
                    continue;
                }

                Vector3 position = new Vector3(-halfX + x, -halfY + xCount - 1 - y, DepthBackground);
                level.slots[x, y].isSlot = true;
                level.slots[x, y].position = position;
                level.slots[x, y].matrix = new Vector2Int(x, y);

                GameObject obj = CreateObject(parent.transform);
                obj.transform.localScale = (new Vector3());
                SpriteRenderer renderer = obj.AddSprite(level.tileBackground);
                m_Seq.Append(obj.transform.DOScale(1, backgroundSpeed));
                renderer.transform.position = position;
            }
        }
    }

    private static void CreateChips()
    {
        LevelData level = LevelManager.instance.level;
        GameObject parent = new GameObject("Tiles");
        parentTile = parent.transform;
        DOTween.SetTweensCapacity(200, xCount* yCount);
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                if (!level.slots[x, y].isSlot) continue;

                m_Seq = DOTween.Sequence();
                GetNewChip(new Vector2Int(x, y));
            }
        }
    }



    private static GameObject CreateObject(Transform parent = null)
    {
        GameObject obj = new GameObject("Cell");
        obj.transform.parent = parent;
        return obj;
    }

    private static SpriteRenderer AddSprite(this GameObject obj, Sprite icon)
    {
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.name = obj.name;
        renderer.sprite = icon;
        return renderer;
    }

    private static BoxCollider2D AddCollider(this GameObject obj)
    {
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size -= new Vector2(0.02f, 0.02f);
        collider.isTrigger = true;
        return collider;
    }

    private static Chip AddChip(this GameObject obj, Vector2Int slotIndex)
    {
        LevelData level = LevelManager.instance.level;
        Chip chip = obj.AddComponent<Chip>();
        chip.id = level.slots[slotIndex.x, slotIndex.y].Slot.id;
        chip.matrix = slotIndex;
        return chip;
    }
}
