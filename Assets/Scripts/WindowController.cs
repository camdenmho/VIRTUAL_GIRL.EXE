using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    public GameObject avatarWindow;
    public GameObject shopWindow;
    public GameObject streamWindow;
    public GameObject hisscordWindow;
    public GameObject readmeWindow;

    public CharacterCustomization customization;

    // Default positions
    public Vector2 avatarDefaultPos = new Vector2(0, 0);
    public Vector2 shopDefaultPos = new Vector2(0,0);
    public Vector2 streamDefaultPos = new Vector2(0,0);
    public Vector2 hisscordDefaultPos = new Vector2(0,0);
    public Vector2 readmeDefaultPos = new Vector2(0,0);
    
    // Open a window
    public void OpenWindow(GameObject window) {
        if (customization != null && !customization.IsInitialized) {
            customization.Initialize();
        }

        // If opening avatar or shop, close the other (both cannot be open at the same time)
        if (window == avatarWindow && shopWindow.activeSelf) {
            shopWindow.SetActive(false);
        }
        else if (window == shopWindow && avatarWindow.activeSelf) {
            avatarWindow.SetActive(false);
        }

        // Open requested window
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
        else if (window = hisscordWindow) {
            rect.anchoredPosition = hisscordDefaultPos;
        }
        else if (window = readmeWindow) {
            rect.anchoredPosition = readmeDefaultPos;
        }
    }
}