using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 offset;
    private RectTransform windowRect;
    
    void Awake() {
        windowRect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRect, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 localPointerPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition)) {
            windowRect.localPosition = localPointerPosition - offset;
        }
    }
}
