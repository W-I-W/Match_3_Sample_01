using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;

using System.Collections;

using UnityEngine;

namespace Bow.Editor
{
    internal sealed class ChipSlotDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, TileSlot> where TArray : IList
    {

        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = true,
                HideRowIndices = true,
                ResizableColumns = false,
            };
        }

        protected override TileSlot DrawElement(Rect rect, TileSlot value)
        {
            int id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, value.slot?.icon, null, id);

            value = DragAndDropUtilities.DropZone(rect, value);
            value.slot = DragAndDropUtilities.DropZone<Tile>(rect, value.slot);
            value = DragAndDropUtilities.DragZone(rect, value, true, true);

            return value;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);
            var rect = GUILayoutUtility.GetRect(0, 20, new GUIStyle());
            var id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, null, null, id);
            DragAndDropUtilities.DropZone(rect, new TileSlot(), false, id);
        }
    }
}
