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
    public TMP_Text resultText;
    public TMP_Text followersText;
    public TMP_Text donationsText;

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

    
    void Start()
    {
        LoadPlayerStats();
        SetDifficulty(selectedDifficulty);
        StartGame();
    }

    #region Player Stats
    void LoadPlayerStats() {
        totalFollowers = PlayerPrefs.GetInt("Followers", 0);
        totalDonations = PlayerPrefs.GetInt("Donations", 0);
        UpdateUI();
    }

    void UpdateUI() {
        followersText.text = "Follwers: " + totalFollowers;
        donationsText.text = "Donations: $" + totalDonations;
    }
    #endregion

    #region Difficulty
    public void SetDifficulty(int index) {
        if (index < 0 || index >= difficulties.Count) return;

        selectedDifficulty = index;
        currentDifficulty = difficulties[selectedDifficulty];
        LoadPhrases();

        timer = currentDifficulty.timeLimit;
    }

    void LoadPhrases() {
        allPhrases.Clear();
        if (currentDifficulty.phraseFile != null) {
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

    public void StartGame() {
        phrasesTyped = 0; // reset counter
        timer = currentDifficulty.timeLimit;
        isPlaying = true;

        inputField.interactable = true;
        inputField.text = "";
        inputField.ActivateInputField();

        resultText.text = "";
        PickNewPhrase();
    }

    private void PickNewPhrase() {
        if (allPhrases.Count == 0) return;

        int index = Random.Range(0, allPhrases.Count);
        currentPhrase = allPhrases[index];
        phraseText.text = currentPhrase;

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void CheckInput() {
        if (!isPlaying) return;

        if (inputField.text == currentPhrase) {
            RewardPlayer();  
            phrasesTyped++;

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
    #endregion

    #region Rewards
    void RewardPlayer() {
        int followerGain = Mathf.RoundToInt(followersPerPhrase * currentDifficulty.rewardMultiplier);
        int donationGain = Mathf.RoundToInt(donationsPerPhrase * currentDifficulty.rewardMultiplier);

        totalFollowers += followerGain;
        totalDonations += donationGain;

        SaveStats();
        UpdateUI();
    }

    void LoseFollowers() {
        totalFollowers = Mathf.Max(totalFollowers - followerPenaltyPerFail, 0);
        SaveStats();
        UpdateUI();
    }

    void SaveStats() {
        PlayerPrefs.SetInt("Followers", totalFollowers);
        PlayerPrefs.SetInt("Donations", totalDonations);
    }
    #endregion


    private void EndGame(bool success) {
        if (!isPlaying) return;

        isPlaying = false; // stop the game
        inputField.interactable = false; // no more typing after the game ends

        resultText.text = success ? "Success!" : "Failed!";
    }
}
