using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform shape;
    public RectTransform bar;
    private Vector2 startMousePosition;
    private Vector2 startShapePosition;
    private bool isMove = false;

    void Awake()
    {
        startShapePosition = shape.position;
        bar = this.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startMousePosition = Input.mousePosition;
        startShapePosition = shape.position;
        isMove = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMove = false;
    }

    void Update()
    {
        if (isMove)
        {
            Vector2 nextPosition = new Vector2(startShapePosition.x - (startMousePosition.x - Input.mousePosition.x), startShapePosition.y - (startMousePosition.y - Input.mousePosition.y));
            if (nextPosition.x <= Camera.main.pixelWidth - shape.rect.width)
            {
                if (nextPosition.x > 0f)
                {
                    shape.position = new Vector2(nextPosition.x, shape.position.y);
                }
                else
                {
                    shape.position = new Vector2(1f, shape.position.y);
                }
            }
            else
            {
                shape.position = new Vector2(Camera.main.pixelWidth - shape.rect.width, shape.position.y);
            }
            
            if (nextPosition.y <= Camera.main.pixelHeight - shape.rect.height)
            {
                if (nextPosition.y >= 0 - (shape.rect.height - bar.rect.height))
                {
                    shape.position = new Vector2(shape.position.x, nextPosition.y);
                }
                else
                {
                    shape.position = new Vector2(shape.position.x, 0 - (shape.rect.height - bar.rect.height));
                }
            }
            else
            {
                shape.position = new Vector2(shape.position.x, Camera.main.pixelHeight - shape.rect.height);
            }
        }
    }
}
