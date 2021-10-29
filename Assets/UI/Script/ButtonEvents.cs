using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Diagnostics;

public class ButtonEvents : MonoBehaviour
{
    enum state
    {
        close = 0,
        open = 1
    }

    enum shapeNames
    {
        Menu = 0
    }

    public Text timeScaleTextObject;
    public List<GameObject> shapes;

    [Header("Menu-Bar")]
    public bool barShowed = false;
    public GridLayoutGroup menuBar; //Панель-Меню справа с низу экрана
    public Sprite swichedSprite;
    private Image switchButton;

    [Header("Cursor")]
    public Texture2D cursor;
    public Texture2D cursorHorisontalScale;
    public Texture2D cursorVerticalScale;
    public Texture2D cursorTopDownScale;
    public Texture2D cursorDownTopScale;

    static public List<Texture2D> cursoreStates = new List<Texture2D>();

    public enum CursorStates
    {
        Default = 0,
        HorisontalScale = 1,
        VerticalScale = 2,
        TopDownScale = 3,
        DownTopScale = 4
    }

    private void Awake()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
        cursoreStates.Add(cursor);
        cursoreStates.Add(cursorHorisontalScale);
        cursoreStates.Add(cursorVerticalScale);
        cursoreStates.Add(cursorTopDownScale);
        cursoreStates.Add(cursorDownTopScale);


        foreach (var currentShape in shapes)
        {
            currentShape.SetActive(false);
        }
    }

    public void OpenGraphApp()
    {
        Process.Start("Graphs.py");
    }

    public void RunTimeAcceleration()
    {
        Time.timeScale += 0.1f;
        timeScaleTextObject.text = (Mathf.Ceil(Time.timeScale * 10) / 10).ToString();
    }

    public void RunTimeDecceleration()
    {
        Time.timeScale -= 0.1f;
        timeScaleTextObject.text = (Mathf.Ceil(Time.timeScale * 10) / 10).ToString();

    }

    public void MenuBarContol()
    {
        if (barShowed)
        {
            HideMenuBar();
        }
        else
        {
            ShowMenuBar();
        }
        barShowed = !barShowed;
    }

    private void ShowMenuBar()
    {
        menuBar.cellSize = new Vector2(210, 30);
        ChangeSprite();
    }

    private void HideMenuBar()
    {
        menuBar.cellSize = new Vector2(30, 30);
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        Sprite temp = switchButton.sprite;
        switchButton.sprite = swichedSprite;
        swichedSprite = temp;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (shapes[(int)shapeNames.Menu].activeSelf)
            {
                shapes[(int)shapeNames.Menu].SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                shapes[(int)shapeNames.Menu].SetActive(true);
                Time.timeScale = 0;
            }
        }
    }


}