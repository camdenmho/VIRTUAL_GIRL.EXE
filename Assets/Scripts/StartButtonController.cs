using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonController : MonoBehaviour
{
    public GameObject powerPanel;

    // Toggle power button panel
    public void OnStartButtonClicked() {
        if (powerPanel != null) {
            bool isActive = powerPanel.activeSelf;
            powerPanel.SetActive(!isActive);
        }
    }
}
