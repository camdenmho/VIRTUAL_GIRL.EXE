using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WpmTest : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text sentenceText;
    [SerializeField] private TMP_InputField typingField;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TextAsset sentenceFile;
    [SerializeField] private GameObject retryButton;

    [Header("Audio")]
    [SerializeField] private AudioController audioController;

    [Header("Typing Highlight Colors")]
    [SerializeField] private string typedColorHex = "#f181d9"; // color for correctly typed characters
    [SerializeField] private string wrongColorHex = "#cd3066"; // color for incorrectly typed characters

    private string targetSentence;
    private float startTime;
    private bool testStarted = false;
    private bool testEnded = false;

    private int previousLength = 0;

    private List<string> allPhrases = new List<string>();

    private void Start() {
        // Listen for input and initialize the test
        typingField.onValueChanged.AddListener(OnTypingChanged);
        PrepareTest();
    }

    private void OnEnable() {
        // Reset test state when window reopens
        PrepareTest();
    }

    // Resets the test state and loads a new sentence
    public void PrepareTest() {
        testStarted = false;
        testEnded = false;
        typingField.text = "";
        typingField.interactable = true;
        resultText.text = "";

        LoadPhrases();
        SelectRandomSentence();
        typingField.ActivateInputField();

        // Hide retry button at start
        if (retryButton != null) {
            retryButton.SetActive(false);
        }

        // Show current high score
        if (highScoreText != null) {
            int savedHighScore = PlayerPrefs.GetInt("HighWPM", 0);
            highScoreText.text = $"High Score: {savedHighScore} WPM";
        }
    }

    // Load phrases from the text file
    private void LoadPhrases() {
        allPhrases.Clear();
        // Split file by newline
        string[] lines = sentenceFile.text.Split('\n');
        
        foreach (string line in lines) {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed)) {
                allPhrases.Add(trimmed);
            }
        }
    }

    // Selects a random sentence from the loaded list
    private void SelectRandomSentence() {
        if (allPhrases.Count == 0) {
            sentenceText.text = "No phrases found";
            targetSentence = "";
            return;
        }

        targetSentence = allPhrases[Random.Range(0, allPhrases.Count)];
        UpdateHighlightedSentence(""); // show full sentence initially (with no highlights)
    }

    // Called when input field changes
    private void OnTypingChanged(string currentText) {
        if (!testStarted && currentText.Length > 0) {
            startTime = Time.time;
            testStarted = true;
        }

        // Typing sound only when a new character is added
        if (currentText.Length > previousLength) {
            audioController?.PlayTyping();
        }
        previousLength = currentText.Length;

        UpdateHighlightedSentence(currentText);

        // End test if sentence typed correctly
        if (!testEnded && currentText == targetSentence) {
            EndTest(currentText.Length);
        }
    }

    // Highlights typed characters
    private void UpdateHighlightedSentence(string typed) {
        string displayText = "";

        for (int i = 0; i < targetSentence.Length; i++) {
            if (i < typed.Length) {
                if (typed[i] == targetSentence[i]) {
                    displayText += $"<color={typedColorHex}>{targetSentence[i]}</color>"; // correct
                }
                else {
                    displayText += $"<color={wrongColorHex}>{targetSentence[i]}</color>"; // incorrect
                }
            }
            else {
                displayText += targetSentence[i]; // not yet typed
            }
        }

        sentenceText.text = displayText;
    }

    // End test, calculate WPM, update high score
    private void EndTest(int charCount) {
        testEnded = true;
        typingField.interactable = false;
        typingField.text = ""; // clear input field after test ends

        float elapsedMinutes = (Time.time - startTime) / 60f;
        float wpm = charCount / 5f / elapsedMinutes; // standard WPM formula
        int finalWpm = Mathf.RoundToInt(wpm);

        resultText.text = $"WPM: {finalWpm}";

        // Update and display high score
        int savedHighScore = PlayerPrefs.GetInt("HighWPM", 0);
        if (finalWpm > savedHighScore) {
            PlayerPrefs.SetInt("HighWPM", finalWpm);
            PlayerPrefs.Save();
            savedHighScore = finalWpm;
        }
        if (highScoreText != null) {
            highScoreText.text = $"High Score: {savedHighScore} WPM";
        }

        // Show retry button
        if (retryButton != null) {
            retryButton.SetActive(true);
        }
    }

    // Reset
    [ContextMenu("Reset PlayerPrefs High WPM")]
    public void ResetHighWpm() {
        PlayerPrefs.SetInt("HighWPM", 0);
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs reset: HighWPM = 0");
    }
}
