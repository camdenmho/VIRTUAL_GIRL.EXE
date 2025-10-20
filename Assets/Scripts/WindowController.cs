using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    public GameObject avatarWindow;
    public GameObject shopWindow;
    public GameObject streamWindow;

    // Default positions
    public Vector2 avatarDefaultPos = new Vector2(976, -552);
    public Vector2 shopDefaultPos = new Vector2(0,0);
    public Vector2 streamDefaultPos = new Vector2(0,0);
    
    // Open a window
    public void OpenWindow(GameObject window) {
        if (window != null) {
            window.SetActive(true);
            ResetWindowPosition(window);
        }
    }

    // Close a window
    public void CloseWindow(GameObject window) {
        if (window != null) {
            window.SetActive(false);
        }
    }

    // Reset window to default position
    private void ResetWindowPosition(GameObject window) {
        RectTransform rect = window.GetComponent<RectTransform>();
        if (rect == null) return;

        if (window == avatarWindow) {
            rect.anchoredPosition = avatarDefaultPos;
        }
        else if (window = shopWindow) {
            rect.anchoredPosition = shopDefaultPos;
        }
        else if (window = streamWindow) {
            rect.anchoredPosition = streamDefaultPos;
        }
    }
}