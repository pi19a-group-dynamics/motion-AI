using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Panel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform bodyTransform;
    private Coroutine currentCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentCoroutine = StartCoroutine(Move());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(currentCoroutine);
    }

    IEnumerator Move()
    {
        Vector3 startMousePosition = Input.mousePosition;
        Vector3 startWindowPosition = bodyTransform.position;
        while (true)
        {
            bodyTransform.position = startWindowPosition - (startMousePosition - Input.mousePosition);
            yield return null;
        }
    }
}
