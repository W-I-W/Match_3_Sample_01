using Bow.Data;

using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public sealed class ChipChecker : MonoBehaviour
{
    private Camera m_Camera;

    private Chip[] m_Chips;

    private Sequence m_Seq;

    private readonly float m_DelayStartChip = 0.0f;
    private readonly float m_DelayCreateChip = 0.12f;

    private LevelData level => LevelManager.instance.level;


    private void Start()
    {
        m_Chips = new Chip[2];
        m_Camera = Camera.main;
    }

    private void OnClick(InputValue value)
    {
        Vector2 mouse = Mouse.current.position.value;
        Vector2 position = m_Camera.ScreenToWorldPoint(mouse);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        int index = System.Convert.ToInt32(value.Get());
        if (hit.transform == null) return;

        bool isChip = hit.transform.TryGetComponent(out Chip chip);
        m_Chips[index] = chip;

        if (!isChip) return;
        if (index == 1) return;
        if (m_Chips[1] == null) return;
        if (m_Chips[0] == m_Chips[1]) return;

        Vector2Int matrixEnd = m_Chips[0].matrix;
        Vector2Int matrixStart = m_Chips[1].matrix;
        int dis = Mathf.Abs((matrixEnd.x + matrixEnd.y) - (matrixStart.x + matrixStart.y));
        if (dis != 1) return;

        m_Seq = DOTween.Sequence();
        m_Seq.Append(Swap(matrixEnd, matrixStart));
        List<Chip> chipsStart = FindMatch(matrixStart);
        List<Chip> chipsEnd = FindMatch(matrixEnd);
        if (chipsStart.Count != 1 || chipsEnd.Count != 1)
        {
            m_Seq.AppendCallback(() => FindAll());
            return;
        }
        m_Seq.Append(Swap(matrixStart, matrixEnd));
    }

    private IEnumerator OnStep(List<Chip> chips)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(TakeDestroy(chips));
        while (seq.active)
            yield return null;

        seq.Kill();

        seq = DOTween.Sequence();
        seq.Append(UpdateChips());
        while (seq.active)
            yield return null;

        seq.Kill();

        seq = DOTween.Sequence();
        seq.Append(AddChips());
        while (seq.active)
            yield return null;

        FindAll();
        yield return null;
    }

    private Sequence Swap(Vector2Int a, Vector2Int b)
    {
        Chip chipStart = level.slots[b.x, b.y].chip;
        Chip chipEnd = level.slots[a.x, a.y].chip;

        chipStart.SetSlot(level.slots[a.x, a.y]);
        chipEnd.SetSlot(level.slots[b.x, b.y]);

        level.slots[b.x, b.y].chip = chipEnd;
        level.slots[a.x, a.y].chip = chipStart;


        Sequence seq = DOTween.Sequence();
        seq.Join(level.slots[a.x, a.y].chip.DoMove());
        seq.Join(level.slots[b.x, b.y].chip.DoMove());

        return seq;
    }

    private Sequence FindAll()
    {
        bool status = false;
        List<Chip> allChips = new List<Chip>();
        for (int x = 0; x < level.size.x; x++)
        {
            for (int y = 0; y < level.size.y; y++)
            {
                if (level.slots[x, y].isSlot == false) continue;
                if (level.slots[x, y].chip == null) continue;

                List<Chip> chips = FindMatch(new Vector2Int(x, y));
                if (chips.Count != 1)
                {
                    allChips.AddRange(chips);
                    status = true;
                }
            }
        }
        if (status == false) return m_Seq;
        var newChips=allChips.Distinct().ToList();
        StartCoroutine(OnStep(newChips));

        return m_Seq;
    }

    private List<Chip> FindMatch(Vector2Int index)
    {
        var chips = new List<Chip>()
        {
            level.slots[index.x,index.y].chip
        };

        List<Chip> row = InRow(index);
        List<Chip> cloumn = InColumn(index);

        if (row.Count > 1)
            chips.AddRange(row);
        if (cloumn.Count > 1)
            chips.AddRange(cloumn);
        if (row.Count > 0 && cloumn.Count > 0)
        {
            var sort = chips.Union(Aslant(index));
            chips = sort.ToList();
        }
        return chips;
    }

    private List<Chip> InRow(Vector2Int index)
    {
        var chips = new List<Chip>();
        Chip chip = level.slots[index.x, index.y].chip;

        int idChip = chip.id;

        for (int x = index.x + 1; x < level.size.x; x++)
        {
            if (level.slots[x, index.y].chip == null) break;
            if (level.slots[x, index.y].chip.id != idChip) break;
            chips.Add(level.slots[x, index.y].chip);
        }
        for (int x = index.x - 1; x >= 0; x--)
        {
            if (level.slots[x, index.y].chip == null) break;
            if (level.slots[x, index.y].chip.id != idChip) break;
            chips.Add(level.slots[x, index.y].chip);
        }
        return chips;
    }

    private List<Chip> InColumn(Vector2Int index)
    {
        var chips = new List<Chip>();
        Chip chip = level.slots[index.x, index.y].chip;

        int idChip = chip.id;


        for (int y = index.y + 1; y < level.size.y; y++)
        {
            if (level.slots[index.x, y].chip == null) break;
            if (level.slots[index.x, y].chip.id != idChip) break;
            chips.Add(level.slots[index.x, y].chip);
        }
        for (int y = index.y - 1; y >= 0; y--)
        {
            if (level.slots[index.x, y].chip == null) break;
            if (level.slots[index.x, y].chip.id != idChip) break;
            chips.Add(level.slots[index.x, y].chip);
        }
        return chips;
    }

    public List<Chip> Aslant(Vector2Int index)
    {
        var chips = new List<Chip>();
        if (level.slots[index.x, index.y].slot == false) return chips;

        chips.AddRange(AcrossAt(index, Vector2Int.down + Vector2Int.left));
        chips.AddRange(AcrossAt(index, Vector2Int.down + Vector2Int.right));
        chips.AddRange(AcrossAt(index, Vector2Int.up + Vector2Int.left));
        chips.AddRange(AcrossAt(index, Vector2Int.up + Vector2Int.right));
        return chips;
    }

    private List<Chip> AcrossAt(Vector2Int index, Vector2Int shift)
    {
        var chips = new List<Chip>();
        int x = (index + shift).x;
        int y = (index + shift).y;
        if (x < 0 || x >= level.size.x) return chips;
        if (y < 0 || y >= level.size.y) return chips;

        if (level.slots[index.x, index.y].slot == false) return chips;
        if (level.slots[index.x + shift.x, index.y + shift.y].slot == false) return chips;
        if (level.slots[index.x + shift.x, index.y].slot == false) return chips;
        if (level.slots[index.x, index.y + shift.y].slot == false) return chips;

        Chip center = level.slots[index.x, index.y].chip;
        Chip aslant = level.slots[index.x + shift.x, index.y + shift.y].chip;
        Chip shiftX = level.slots[index.x + shift.x, index.y].chip;
        Chip shiftY = level.slots[index.x, index.y + shift.y].chip;

        if (aslant == null) return chips;
        if (shiftX == null) return chips;
        if (shiftY == null) return chips;

        if (center.id != aslant.id) return chips;
        if (center.id != shiftX.id) return chips;
        if (center.id != shiftY.id) return chips;

        return new List<Chip>() { aslant, shiftX, shiftY };
    }

    private Tween UpdateChips()
    {
        Sequence seq = DOTween.Sequence();
        int maxX = level.size.x;
        int maxY = level.size.y;

        for (int x = 0; x < maxX; x++)
        {
            for (int y = maxY - 1; y >= 0; y--)
            {
                if (level.slots[x, y].isSlot == false) continue;
                if (level.slots[x, y].chip != null) continue;
                for (int i = y; i >= 0; i--)
                {
                    if (level.slots[x, i].chip == null) continue;

                    level.slots[x, y].chip = level.slots[x, i].chip;
                    level.slots[x, y].chip.SetSlot(level.slots[x, y]);
                    seq.Join(level.slots[x, y].chip.DoMove());

                    level.slots[x, i].chip = null;
                    break;
                }
            }
        }
        return seq;
    }

    private Tween AddChips()
    {
        Sequence seq = DOTween.Sequence().SetDelay(m_DelayStartChip);
        //m_Seq = DOTween.Sequence();
        int maxX = level.size.x;
        int maxY = level.size.y;
        for (int x = 0; x < maxX; x++)
        {
            seq = DOTween.Sequence();
            for (int y = maxY - 1; y >= 0; y--)
            {
                if (level.slots[x, y].isSlot == false) continue;
                if (level.slots[x, y].chip != null) continue;

                Chip chip = LevelGenerator.pool.Get();

                Vector2 position = level.slots[x, 0].position + Vector2.up;
                chip.SetPosition(position);
                int indexChip = Random.Range(0, level.sizeChip);
                chip.renderer.sprite = level.chips[indexChip].slot.icon;
                chip.id = indexChip;
                chip.SetSlot(level.slots[x, y], m_DelayCreateChip);//.SetDelay(1);
                level.slots[x, y].chip = chip;
                seq.Append(level.slots[x, y].chip.DoMove());
            }
        }
        return seq;
    }

    private Sequence TakeDestroy(List<Chip> chips)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = chips.Count - 1; i >= 0; i--)
        {
            if (level.slots[chips[i].matrix.x, chips[i].matrix.y].chip == null)
            {
                chips.RemoveAt(i);
                continue;
            }
            seq.Join(level.slots[chips[i].matrix.x, chips[i].matrix.y].chip.transform.DOScale(0, 0.5f));
        }
        
        seq.AppendCallback(() =>
        {
            for (int i = chips.Count - 1; i >= 0; i--)
            { 
                LevelGenerator.pool.Release(level.slots[chips[i].matrix.x, chips[i].matrix.y].chip);
                level.slots[chips[i].matrix.x, chips[i].matrix.y].chip = null;
                chips.RemoveAt(i);
            }
        });
        return seq;
    }
}
