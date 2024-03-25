using Bow.Data;

using DG.Tweening;

using Microsoft.Unity.VisualStudio.Editor;

using UnityEngine;
using UnityEngine.Pool;

public class LevelGenerator
{
    public LevelGenerator(LevelData level)
    {
        LevelManager.instance.Init(level);
    }

    //public static LevelGenerator instance;
    public static IObjectPool<Chip> pool { get; set; }
    private Sequence m_Seq;

    private readonly int DepthBackground = 10;
    private readonly int DepthTile = -10;
    private readonly float HalfUnit = 0.5f;


    public float backgroundSpeed { get; set; } = 0.01f;
    public float tileSpeed { get; set; } = 0.4f;
    public Ease tileEase { get; set; } = Ease.OutFlash;

    public Chip[,] chips { get; set; }

    private int xCount => LevelManager.instance.level.slots.GetLength(0);

    private int yCount => LevelManager.instance.level.slots.GetLength(1);

    private float halfX => xCount / 2f - HalfUnit;

    private float halfY => yCount / 2f - HalfUnit;

    private Transform parentTile { get; set; }


    public void Play()
    {
        CreateBackground();
        m_Seq.AppendCallback(() => CreateChips());
    }

    private Chip InstantiateChip()
    {
        GameObject obj = CreateObject(parentTile);
        obj.transform.localScale = new Vector3();
        SpriteRenderer renderer = AddSprite(obj);
        BoxCollider2D collider = AddCollider(obj);
        Chip chip = AddChip(obj);
        chip.renderer = renderer;
        chip.collider = collider;

        m_Seq.Append(obj.transform.DOScale(1, tileSpeed)
            .SetDelay(Random.Range(0f, 0.2f))
            .SetEase(tileEase));
        return chip;
    }

    private void OnChip(Chip chip)
    {
        //LevelData level = LevelManager.instance.level;
        //Chip chip = level.slots[matrix.x, matrix.y].chip;
        //Vector2 position = level.slots[matrix.x, matrix.y].position;
        //chip.Init();
        chip.gameObject.SetActive(true);

    }

    private void OnReleased(Chip chip)
    {
        //LevelData level = LevelManager.instance.level;
        //Chip chip = level.slots[matrix.x, matrix.y].chip;
        chip.gameObject.SetActive(false);
    }


    private void CreateBackground()
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
                SpriteRenderer renderer = AddSprite(obj);
                renderer.sprite = level.tileBackground;
                m_Seq.Append(obj.transform.DOScale(1, backgroundSpeed));
                renderer.transform.position = position;
            }
        }
    }

    private void CreateChips()
    {
        LevelData level = LevelManager.instance.level;
        GameObject parent = new GameObject("Tiles");
        pool = new ObjectPool<Chip>(InstantiateChip, OnChip, OnReleased);
        parentTile = parent.transform;

        DOTween.SetTweensCapacity(200, xCount * yCount);
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                if (!level.slots[x, y].isSlot) continue;

                m_Seq = DOTween.Sequence();
                Vector2 position = level.slots[x, y].position;
                Chip chip = InstantiateChip();
                chip.id = level.slots[x, y].Slot.id;
                chip.matrix = level.slots[x, y].matrix;
                chip.renderer.sprite = level.slots[x, y].Slot.icon;
                chip.Init(new Vector3(position.x, position.y, DepthTile));
                level.slots[x, y].chip = chip;
            }
        }
    }

    private GameObject CreateObject(Transform parent = null)
    {
        GameObject obj = new GameObject("Cell");
        obj.transform.parent = parent;
        return obj;
    }

    private SpriteRenderer AddSprite(GameObject obj)
    {
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        return renderer;
    }

    private BoxCollider2D AddCollider(GameObject obj)
    {
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.size -= new Vector2(0.02f, 0.02f);
        collider.isTrigger = true;
        return collider;
    }

    private Chip AddChip(GameObject obj)
    {
        Chip chip = obj.AddComponent<Chip>();
        return chip;
    }
}
