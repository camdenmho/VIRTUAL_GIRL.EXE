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

    [Header("Audio")]
    public AudioController audioController;


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

        // Start countdown
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

        // Stop and clear chat at the end of each stream
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

            // Play countdown tick sound
            audioController?.PlayCountdownTick();

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        audioController?.PlayCountdownTick();
        countdownText.text = "stream start!";
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);

        // Start stream after countdown ends
        if (typingGame != null) {
            typingGame.StartGame();
        }
        // Start chat after countdown ends
        if (chatSimulator != null) {
            chatSimulator.StartChat();
        }
    }
}
