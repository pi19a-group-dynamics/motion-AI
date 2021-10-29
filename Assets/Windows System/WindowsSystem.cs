using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsSystem : MonoBehaviour
{
    public enum CursorType {
        Default,
        Horizontal,
        Vertical,
        Diagonal,
        Diagonal_Inversion
    }

    [Header("Курсор")]
    [SerializeField] public Texture2D Default;
    [SerializeField] public Texture2D Horizontal;
    [SerializeField] public Texture2D Vertical;
    [SerializeField] public Texture2D Diagonal;
    [SerializeField] public Texture2D Diagonal_Inversion;

    static public WindowsSystem system;

    private void Awake()
    {
        system = this;
        Cursor.SetCursor(Default, Vector2.zero, CursorMode.Auto);
    }

    public Texture2D GetCursor(CursorType type)
    {
        switch (type.ToString())
        {
            case "Default":
                return Default;
            case "Horizontal":
                return Horizontal;
            case "Vertical":
                return Vertical;
            case "Diagonal":
                return Diagonal;
            case "Diagonal_Inversion":
                return Diagonal_Inversion;
        }
        return null;
    }
}
