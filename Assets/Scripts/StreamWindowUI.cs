using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StreamWindowUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text[] followersTexts;
    public TMP_Text[] donationsTexts;

    [Header("Animation Settings")]
    public float countAnimDuration = 0.5f; // seconds

    private int currentFollowers;
    private int currentDonations;

    private Coroutine followersAnimCoroutine;
    private Coroutine donationsAnimCoroutine;

    // Ensures follower and donations are updated
    private void OnEnable() {
        TypingGame.OnStatsUpdated += AnimateStatusUpdate;
        AnimateStatusUpdate();
    }

    private void OnDisable() {
        TypingGame.OnStatsUpdated -= AnimateStatusUpdate;
    }

    // Animate follower and donation numbers
    private void AnimateStatusUpdate() {
        int targetFollowers = PlayerPrefs.GetInt("Followers", 0);
        int targetDonations = PlayerPrefs.GetInt("Donations", 0);

        // Stop any running animations
        if (followersAnimCoroutine != null) {
            StopCoroutine(followersAnimCoroutine);
        }
        if (donationsAnimCoroutine != null) {
            StopCoroutine(donationsAnimCoroutine);
        }

        // Start new animations
        followersAnimCoroutine = StartCoroutine(AnimateNumber(currentFollowers, targetFollowers, countAnimDuration, UpdateFollowersTexts));
        donationsAnimCoroutine = StartCoroutine(AnimateNumber(currentDonations, targetDonations, countAnimDuration, UpdateDonationsTexts));

        // Update current values
        currentFollowers = targetFollowers;
        currentDonations = targetDonations;
    }

    // Coroutine to animate numbers
    private IEnumerator AnimateNumber(int from, int to, float duration, System.Action<int> updateCallback) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int value = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            updateCallback?.Invoke(value);
            yield return null;
        }
        updateCallback?.Invoke(to); // ensure exact final value
    }

    // Update all follower displays
    private void UpdateFollowersTexts(int value) {
        foreach (var text in followersTexts) {
            if (text != null) {
                text.text = value.ToString("N0");
            }
        }
    }

    // Update all donations displays 
    private void UpdateDonationsTexts(int value) {
        foreach (var text in donationsTexts) {
            if (text != null) {
                text.text = "$" + value.ToString("N0");
            }
        }
    }
}
