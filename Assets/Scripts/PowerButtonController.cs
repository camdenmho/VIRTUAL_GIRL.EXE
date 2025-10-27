using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerButtonController : MonoBehaviour
{
    // Quit game
    public void OnPowerButtonClicked() {
        Application.Quit();
    }
}
