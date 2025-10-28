using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HisscordWindowController : MonoBehaviour
{
    public GameObject maximizeMessagePanel;

    // Maximize message for gameplay
    public void OnWpmButtonClicked() {
        if (maximizeMessagePanel != null) {
            maximizeMessagePanel.SetActive(true);
        }
    }

    // Hide the maximize message panel
    public void OnExitButtonClicked() {
        if (maximizeMessagePanel != null) {
            maximizeMessagePanel.SetActive(false);
        }
    }
}
