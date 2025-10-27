using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StreamWindowController : MonoBehaviour
{
    [Header("Overlay Windows")]
    public GameObject gameplayWindow;
    public GameObject countdownPanel;
    public TMP_Text countdownText;
    public ChatSimulator chatSimulator;

    [Header("Countdown Settings")]
    public float countdownSeconds = 3f; // seconds
    public TypingGame typingGame;


    void Start()
    {
        // Hide the Gameplay Window initially
        if (gameplayWindow != null) {
            gameplayWindow.SetActive(false);
        }

        // Hide countdown initially
        if (countdownPanel != null) {
            countdownPanel.SetActive(false);
        }
    }

    // When Start Stream button is clicked
    public void StartStream() {
        if (gameplayWindow != null) {
            gameplayWindow.SetActive(true);
        }

        // Hide results window before countdown begins
        if (typingGame != null && typingGame.resultsWindow != null) {
            typingGame.resultsWindow.SetActive(false);
        }

        if (countdownPanel != null && countdownText != null) {
            StartCoroutine(CountdownCoroutine());
        }
        else {
            typingGame.StartGame();
        }
    }

    // Stop stream
    public void StopStream() {
        if (gameplayWindow != null) {
            gameplayWindow.SetActive(false);
        }

        if (chatSimulator != null) {
            chatSimulator.StopChat();
            chatSimulator.ClearChat();
        }
    }

    // Countdown 
    private IEnumerator CountdownCoroutine() {
        countdownPanel.SetActive(true);
        float timer = countdownSeconds;

        while (timer > 0) {
            countdownText.text = Mathf.Ceil(timer).ToString();
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        countdownText.text = "stream start!";
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);

        if (typingGame != null) {
            typingGame.StartGame();
        }

        if (chatSimulator != null) {
            chatSimulator.StartChat();
        }
    }
}
