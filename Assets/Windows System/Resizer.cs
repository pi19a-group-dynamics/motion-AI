using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    enum HorisontalDirection
    {
        none = 0,
        left = -1,
        right = 1
    }
    enum VerticalDirection
    {
        none = 0,
        up = 1,
        down = -1
    }


    [SerializeField] private WindowsSystem.CursorType CursurType;
    [SerializeField] private RectTransform Body;
    [SerializeField] private Vector2 pivot;
    [SerializeField] private VerticalDirection verticalEdge;
    [SerializeField] private HorisontalDirection horizontalEdge;
    private List<Coroutine> currentCoroutines = new List<Coroutine>();
    private Vector2 position;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Texture2D texture = WindowsSystem.system.GetCursor(CursurType);
        Cursor.SetCursor(texture, new Vector2(texture.width / 2, texture.height / 2), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(WindowsSystem.system.GetCursor(WindowsSystem.CursorType.Default), new Vector2(0, 0), CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (Coroutine currentCoroutine in currentCoroutines) {
            StopCoroutine(currentCoroutine);
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        position = Body.anchoredPosition;
        Vector2 difference = pivot - Body.pivot;
        Body.pivot = pivot;
        Body.anchoredPosition = new Vector2(Body.anchoredPosition.x + Body.rect.width * difference.x, Body.anchoredPosition.y + Body.rect.height * difference.y);

        Vector2 startPosition = Input.mousePosition;
        if (verticalEdge != VerticalDirection.none)
            currentCoroutines.Add(StartCoroutine(VerticalScale(startPosition)));
        if (horizontalEdge != HorisontalDirection.none)
            currentCoroutines.Add(StartCoroutine(HorizontalScale(startPosition)));
    }

    IEnumerator VerticalScale(Vector2 startPosition)
    {
        float startHeight = Body.rect.height;
        while (true)
        {
            Body.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startHeight - (int)verticalEdge * (startPosition.y - Input.mousePosition.y));
            Debug.Log($"{startHeight - (int)verticalEdge * (startPosition.y - Input.mousePosition.y)} . {startPosition.y - Input.mousePosition.y}");
            yield return null;
        }
    }
    
    IEnumerator HorizontalScale(Vector2 startPosition)
    {
        float startWidth = Body.rect.width;
        while (true)
        {
            Body.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startWidth - (int)horizontalEdge * (startPosition.x - Input.mousePosition.x));
            yield return null;
        }
    }
}
