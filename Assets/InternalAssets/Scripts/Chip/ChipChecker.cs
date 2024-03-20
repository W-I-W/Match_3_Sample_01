using Bow;
using Bow.Data;

using System;
using System.Collections.Generic;

using Unity.VisualScripting;

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

        int sizeChipStart = m_Chips[0].index.x + m_Chips[0].index.y;
        int sizeChipEnd = m_Chips[1].index.x + m_Chips[1].index.y;

        if (Mathf.Abs(sizeChipStart - sizeChipEnd) != 1) return;

        LevelData level = LevelManager.instance.level;
        Vector2Int end = m_Chips[0].index;
        Vector2Int start = m_Chips[1].index;

        Vector2 posEnd = m_Chips[0].position;
        Vector2 posStart = m_Chips[1].position;

        level.map.cells[start.x, start.y].renderer.index = end;
        level.map.cells[end.x, end.y].renderer.index = start;

        level.map.cells[start.x, start.y].renderer.position = posEnd;
        level.map.cells[end.x, end.y].renderer.position = posStart;

        level.map.cells[start.x, start.y].renderer = m_Chips[0];
        level.map.cells[end.x, end.y].renderer = m_Chips[1];


        List<Chip> chips = FindMatch(end);
        Debug.Log(chips.Count);
        if (chips.Count != 1)
        {
            for (int i = chips.Count - 1; i >= 0; i--)
            {
                level.map.cells[chips[i].index.x, chips[i].index.y].renderer = null;
                Destroy(chips[i].gameObject);
                chips.RemoveAt(i);
            }
        }
        UpdateChips();
        AddChips();
    }

    private List<Chip> FindMatch(Vector2Int index)
    {
        LevelData level = LevelManager.instance.level;

        List<Chip> chips = new List<Chip>()
        {
            level.map.cells[index.x,index.y ].renderer
        };

        List<Chip> row = InRow(index);
        List<Chip> cloumn = InColumn(index);

        if (row.Count > 1)
            chips.AddRange(row);
        if (cloumn.Count > 1)
            chips.AddRange(cloumn);

        return chips;
    }

    private List<Chip> InRow(Vector2Int index)
    {
        LevelData level = LevelManager.instance.level;
        List<Chip> chips = new List<Chip>();
        Chip chip = level.map.cells[index.x, index.y].renderer;

        int idChip = chip.id;

        for (int x = index.x + 1; x < level.size.x; x++)
        {
            if (level.map.cells[x, index.y].renderer == null) break;
            if (level.map.cells[x, index.y].renderer.id != idChip) break;
            chips.Add(level.map.cells[x, index.y].renderer);
        }
        for (int x = index.x - 1; x >= 0; x--)
        {
            if (level.map.cells[x, index.y].renderer == null) break;
            if (level.map.cells[x, index.y].renderer.id != idChip) break;
            chips.Add(level.map.cells[x, index.y].renderer);
        }
        return chips;
    }

    private List<Chip> InColumn(Vector2Int index)
    {
        LevelData level = LevelManager.instance.level;
        List<Chip> chips = new List<Chip>();
        Chip chip = level.map.cells[index.x, index.y].renderer;
        int idChip = chip.id;


        for (int y = index.y + 1; y < level.size.y; y++)
        {
            if (level.map.cells[index.x, y].renderer == null) break;
            if (level.map.cells[index.x, y].renderer.id != idChip) break;
            chips.Add(level.map.cells[index.x, y].renderer);
        }
        for (int y = index.y - 1; y >= 0; y--)
        {
            if (level.map.cells[index.x, y].renderer == null) break;
            if (level.map.cells[index.x, y].renderer.id != idChip) break;
            chips.Add(level.map.cells[index.x, y].renderer);
        }
        return chips;
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
                if (level.map.cells[x, y].renderer != null) continue;

                for (int up = y; up < countColumn; up++)
                {
                    if (level.map.cells[x, up].renderer == null) continue;

                    level.map.cells[x, y].renderer = level.map.cells[x, up].renderer;
                    level.map.cells[x, y].renderer.index = new Vector2Int(x, y);
                    level.map.cells[x, y].renderer.position = level.map.cells[x, y].position;
                    level.map.cells[x, up].renderer = null;
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
                if (level.map.cells[x, y].renderer != null) continue;
                level.GetNewChip(new Vector2Int(x, y));
            }
        }
    }
}
