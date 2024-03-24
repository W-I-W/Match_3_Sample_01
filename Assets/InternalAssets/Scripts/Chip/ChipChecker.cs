using Bow;
using Bow.Data;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

public class ChipChecker : MonoBehaviour
{
    private Camera m_Camera;

    private Chip[] m_Chips;


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
        int index = Convert.ToInt32(value.Get());
        if (hit.transform == null) return;

        bool isChip = hit.transform.TryGetComponent(out Chip chip);
        m_Chips[index] = chip;

        if (!isChip) return;
        if (index == 1) return;
        if (m_Chips[1] == null) return;
        if (m_Chips[0] == m_Chips[1]) return;

        LevelData level = LevelManager.instance.level;
        Vector2Int matrixEnd = m_Chips[0].matrix;
        Vector2Int matrixStart = m_Chips[1].matrix;
        int dis = Mathf.Abs((matrixEnd.x + matrixEnd.y) - (matrixStart.x + matrixStart.y));
        if (dis != 1) return;

        Chip chipStart = level.slots[matrixStart.x, matrixStart.y].chip;
        Chip chipEnd = level.slots[matrixEnd.x, matrixEnd.y].chip;

        chipStart.SetSlot(level.slots[matrixEnd.x, matrixEnd.y]);
        chipEnd.SetSlot(level.slots[matrixStart.x, matrixStart.y]);

        level.slots[matrixStart.x, matrixStart.y].chip = chipEnd;
        level.slots[matrixEnd.x, matrixEnd.y].chip = chipStart;


        List<Chip> m_Tiles = FindMatch(matrixEnd);
        Debug.Log(m_Tiles.Count);
        //if (m_Tiles.Count != 1)
        //{
        //    for (int i = m_Tiles.Count - 1; i >= 0; i--)
        //    {
        //        level.slots[m_Tiles[i].matrix.x, m_Tiles[i].matrix.y].center = null;
        //        Destroy(m_Tiles[i].gameObject);
        //        m_Tiles.RemoveAt(i);
        //    }
        //}
        //UpdateChips();
        //AddChips();
    }

    private List<Chip> FindMatch(Vector2Int index)
    {
        LevelData level = LevelManager.instance.level;

        List<Chip> chips = new List<Chip>()
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
        LevelData level = LevelManager.instance.level;
        List<Chip> chips = new List<Chip>();
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
        LevelData level = LevelManager.instance.level;
        List<Chip> chips = new List<Chip>();
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
        List<Chip> chips = new List<Chip>();
        chips.AddRange(AcrossAt(index, Vector2Int.down + Vector2Int.left));
        chips.AddRange(AcrossAt(index, Vector2Int.down + Vector2Int.right));
        chips.AddRange(AcrossAt(index, Vector2Int.up + Vector2Int.left));
        chips.AddRange(AcrossAt(index, Vector2Int.up + Vector2Int.right));
        return chips;
    }

    private List<Chip> AcrossAt(Vector2Int index, Vector2Int shift)
    {
        List<Chip> chips = new List<Chip>();
        LevelData level = LevelManager.instance.level;
        int x = (index + shift).x;
        int y = (index + shift).x;
        if (x < 0 || x >= level.size.x) return chips;
        if (y < 0 || y >= level.size.y) return chips;

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

    private void UpdateChips()
    {
        LevelData level = LevelManager.instance.level;
        int countRow = level.size.x;
        int countColumn = level.size.y;
        for (int x = 0; x < countRow; x++)
        {
            for (int y = 0; y < countColumn; y++)
            {
                if (level.slots[x, y].chip != null) continue;

                for (int up = y; up < countColumn; up++)
                {
                    if (level.slots[x, up].chip == null) continue;

                    level.slots[x, y].chip = level.slots[x, up].chip;
                    level.slots[x, y].chip.matrix = new Vector2Int(x, y);
                    level.slots[x, y].chip.position = level.slots[x, y].position;
                    level.slots[x, up].chip = null;
                    break;
                }
            }
        }
    }

    private void AddChips()
    {
        LevelData level = LevelManager.instance.level;
        int countRow = level.size.x;
        int countColumn = level.size.y;

        for (int x = 0; x < countRow; x++)
        {
            for (int y = 0; y < countColumn; y++)
            {
                if (level.slots[x, y].chip != null) continue;
                if (level.slots[x, y].isSlot == false) continue;
                LevelGenerator.GetNewChip(new Vector2Int(x, y));
            }
        }
    }
}
