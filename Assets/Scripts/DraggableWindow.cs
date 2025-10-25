using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 offset; // Distance between pointer and window pivot on drag
    private RectTransform windowRect; // RectTransform of the window being dragged
    
    void Awake() {
        windowRect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Bring clicked window to the front
        windowRect.SetAsLastSibling();
        
        // Calculates offset between pointer position and window position and keeps cursor in the same spot
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRect, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 localPointerPosition;

        // Convert pointer position to local coordinates revlative to parent canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(windowRect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition)) {
            // Set window position based on pointer position
            windowRect.localPosition = localPointerPosition - offset;
        }
    }
}
