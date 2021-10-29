using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeSizeControlle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Axles
    {
        Horisontal,
        Vertical,
        Hybrid
    }

    public enum Side
    {
        Top = 0,
        TopLeft = 1,
        Left = 2,
        BottomLeft = 3,
        Bottom = 4,
        BottomRight = 5,
        Right = 6,
        TopRight = 7
    }


    static private CursorMode cursorMode = CursorMode.ForceSoftware;
    public ButtonEvents.CursorStates State;
    public Axles axles;
    public Side pivotSide;
    static public Vector2 pivot;
    public bool isResize;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(ButtonEvents.cursoreStates[(int)State], new Vector2(8f, 8f), cursorMode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(ButtonEvents.cursoreStates[(int)ButtonEvents.CursorStates.Default], Vector2.zero, cursorMode);
    }

    private void Update()
    {
        
    }
}
