using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingGame : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text phraseText;
    public TMP_InputField inputField;
    public TMP_Text timerText;
    [HideInInspector] public TMP_Text followersText;
    [HideInInspector] public TMP_Text donationsText;
    public ChatSimulator chatSimulator;

    [Header("Round Info")]
    public TMP_Text roundInfoText;
    private int currentRound = 1;

    [Header("Results Window")]
    public GameObject resultsWindow;
    public TMP_Text resultsFollowersText;
    public TMP_Text resultsDonationsText;

    [Header("Typing Highlight Colors")]
    public string typedColorHex = "#f181d9"; // color for correctly typed characters
    public string wrongColorHex = "#cd3066"; // color for incorrectly typed characters

    [System.Serializable]
    public class Difficulty {
        public string name; // "Easy", "Medium", "Hard"
        public TextAsset phraseFile; // text file for phrases
        public float timeLimit; // seconds to type the phrase
        public int phrasesToType = 5;
        public float rewardMultiplier = 1f; // multiplies followers and donations
    }

    [Header("Difficulty Settings")]
    public List<Difficulty> difficulties;
    public int selectedDifficulty = 0; // 0 = Easy, 1 = Medium, 2 = Hard

    private Difficulty currentDifficulty;
    private List<string> allPhrases = new List<string>();
    private string currentPhrase;

    private int phrasesTyped = 0;
    private float timer;
    private bool isPlaying = false;

    [Header("Rewards")]
    public int followersPerPhrase = 10; // base amount per successful phrase
    public int donationsPerPhrase = 5; // base donation amount per successful phrase
    public int followerPenaltyPerFail = 5; // followers lost per fail (cannot go below 0)

    private int totalFollowers;
    private int totalDonations;
    private int roundFollowers; // followers gained in one round
    private int roundDonations; // donations gained in one round

    public static System.Action OnStatsUpdated;
    
    void Start()
    {
        LoadPlayerStats();
        SetDifficulty(selectedDifficulty);
        StartGame();

        // Update highlighting as player types
        inputField.onValueChanged.AddListener(delegate {UpdatedPhraseDisplay(); });
    }

    #region Player Stats
    // Load player stats from PlayerPrefs
    void LoadPlayerStats() {
        totalFollowers = PlayerPrefs.GetInt("Followers", 0);
        totalDonations = PlayerPrefs.GetInt("Donations", 0);
        currentRound = Mathf.Max(PlayerPrefs.GetInt("Round", 1),1);
    }
    #endregion

    #region Difficulty
    // Sets difficulty level by round (Easy, medium, hard)
    public void SetDifficulty(int round) {
        if (round >= 0 && round <= 3) {
            selectedDifficulty = 0; // easy
        }
        else if (round >= 4 && round <= 6) {
            selectedDifficulty = 1; // medium
        }
        else {
            selectedDifficulty = 2; // hard
        }
    
        currentDifficulty = difficulties[selectedDifficulty];
        LoadPhrases();
        timer = currentDifficulty.timeLimit;

        // Update round display
        if (roundInfoText != null) {
            string difficultyName = currentDifficulty.name.ToUpper();
            roundInfoText.text = $"Round: {currentRound}, {difficultyName}";
        }
    }

    // Reads phrases from the difficulty's text file
    void LoadPhrases() {
        allPhrases.Clear();
        if (currentDifficulty.phraseFile != null) {
            // Split file per line
            string[] lines = currentDifficulty.phraseFile.text.Split('\n');
            foreach (string line in lines) {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed)) {
                    allPhrases.Add(trimmed);
                }
            }
        }
        else {
            Debug.LogError("Phrases file missing for difficulty: " + currentDifficulty.name);
        }
    }
    #endregion

    #region Game Flow
    void Update()
    {
        if (!isPlaying) return;

        // Countdown timer
        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

        if (timer <= 0) {
            EndGame(false); // fail if time runs out
        }
    }

    // Starts a new round
    public void StartGame() {
        SetDifficulty(currentRound); // set difficulted based on round

        phrasesTyped = 0; // reset counter
        timer = currentDifficulty.timeLimit;
        isPlaying = true;

        roundFollowers = 0;
        roundDonations = 0;

        // Clear chat at round start
        if (chatSimulator != null) {
            chatSimulator.ClearChat(); 
        }

        // Enable typing
        inputField.interactable = true;
        inputField.text = "";
        inputField.ActivateInputField();

        if (resultsWindow != null) {
            resultsWindow.SetActive(false);
        }

        PickNewPhrase();
    }

    // Picks a random phrase from the list
    private void PickNewPhrase() {
        if (allPhrases.Count == 0) return;

        int index = Random.Range(0, allPhrases.Count);
        currentPhrase = allPhrases[index];

        // clear input field before updating highlight
        inputField.text = "";
        inputField.ActivateInputField();
        
        UpdatedPhraseDisplay(); // show phrase with highlight
    }

    // Called when player presses Enter
    public void CheckInput() {
        if (!isPlaying) return;

        // Checks if input matches the target phrase
        if (inputField.text == currentPhrase) {
            RewardPlayer();  
            phrasesTyped++;

            // If required phrases are complete, end round
            if (phrasesTyped >= currentDifficulty.phrasesToType) {
                EndGame(true); // correct
            }
            else {
                PickNewPhrase();
            }
        }
        else {
            LoseFollowers();
        }
    }

    // Highlights typed characters
    private void UpdatedPhraseDisplay() {
        if (string.IsNullOrEmpty(currentPhrase) || inputField == null) return;

        string typed = inputField.text;
        string displayText = "";

        for (int i = 0; i < currentPhrase.Length; i++) {
            if (i < typed.Length) {
                if (typed[i] == currentPhrase[i]) {
                    displayText += $"<color={typedColorHex}>{currentPhrase[i]}</color>"; // correct
                }
                else {
                    displayText += $"<color={wrongColorHex}>{currentPhrase[i]}</color>"; // incorrect
                }
            }
            else {
                displayText += currentPhrase[i]; // not yet typed
            }
        }
        phraseText.text = displayText;
    }
    #endregion

    #region Rewards
    // When player types a phrase correctly
    void RewardPlayer() {
        int followerGain = Mathf.RoundToInt(followersPerPhrase * currentDifficulty.rewardMultiplier);
        int donationGain = Mathf.RoundToInt(donationsPerPhrase * currentDifficulty.rewardMultiplier);

        totalFollowers += followerGain;
        totalDonations += donationGain;

        roundFollowers += followerGain;
        roundDonations += donationGain;

        SaveStats();
    }

    // When player types phrase incorrectly
    void LoseFollowers() {
        totalFollowers = Mathf.Max(totalFollowers - followerPenaltyPerFail, 0);
        SaveStats();
    }

    // Saves current follower and donation totals to PlayerPrefs
    void SaveStats() {
        PlayerPrefs.SetInt("Followers", totalFollowers);
        PlayerPrefs.SetInt("Donations", totalDonations);
        PlayerPrefs.SetInt("Round", currentRound);
        PlayerPrefs.Save();

        OnStatsUpdated?.Invoke();
    }
    #endregion

    // End game round and show results
    private void EndGame(bool success) {
        if (!isPlaying) return;

        isPlaying = false; // stop the game
        inputField.interactable = false; // no more typing after the game ends

        // Clear input
        inputField.text = "";

        // Stop chat when round ends
        if (chatSimulator != null) {
            chatSimulator.StopChat();
        }

        // Penalty if player failed
        if (!success) {
            totalFollowers = Mathf.Max(totalFollowers - 100, 0);
            roundFollowers -= 100;
            SaveStats();
        }

        // Display results window
        if (resultsWindow != null) {
            resultsWindow.SetActive(true);
            if (resultsFollowersText != null) {
                // + or - based on followers gained or lost
                string prefix = roundFollowers >= 0 ? "+" : "-";
                resultsFollowersText.text = prefix + Mathf.Abs(roundFollowers).ToString("N0");
            }
            if (resultsDonationsText != null) {
                resultsDonationsText.text = "+$" + roundDonations.ToString("N0");
            }
        }

        // Only increase round if player suceeded
        if (success) {
            currentRound++;
        }
        SaveStats();
    }

    [ContextMenu("Reset PlayerPrefs Followers and Donations")]
    public void ResetFollowersAndDonations() {
        PlayerPrefs.SetInt("Followers", 0);
        PlayerPrefs.SetInt("Donations", 0);
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs reset: Followers: 0, Donations = 0");
    }

    [ContextMenu("Reset PlayerPrefs Round")]
    public void ResetRound() {
        PlayerPrefs.SetInt("Round", 1);
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs reset: Round = 1");
    }
}
