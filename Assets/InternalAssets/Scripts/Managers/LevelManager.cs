using Bow.Data;


using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; set; }

    public LevelData level { get; private set; }

    public UnityAction<Vector2Int> onGenerate { get; set; }

    private void OnValidate()
    {
        instance = this;
    }

    public void SelectLevel(LevelData level)
    {
        this.level = level;
        onGenerate?.Invoke(level.size);
    }
}
