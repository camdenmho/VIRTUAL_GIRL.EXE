using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCustomization : MonoBehaviour
{
    [Header("Avatar Layers")]
    [HideInInspector] public Image outfitImage;
    [HideInInspector] public Image hairImage;

    [Header("Options")]
    public Sprite[] outfitOptions;
    public Sprite[] hairOptions;

    [Header("Default Unlocks")]
    [Tooltip("Indices of outfits unlocked by default")]
    public int[] defaultUnlockedOutfits;
    [Tooltip("Indices of hairs unlocked by default")]
    public int[] defaultUnlockedHairs;

    [Header("Unlock Status")]
    private bool[] unlockedOutfits;
    private bool[] unlockedHairs;

    [Header("Pricing")]
    public int[] outfitPrices;
    public int[] hairPrices;

    [Header("Avatar Window Titles")]
    public TMP_Text outfitTitleText;
    public TMP_Text hairTitleText;

    [Header("Shop Window Titles")]
    public TMP_Text shopOutfitTitleText;
    public TMP_Text shopHairTitleText;
    public TMP_Text totalCostText;
    public TMP_Text outfitPriceText;
    public TMP_Text hairPriceText;
    public TMP_Text donationTotalText;
    public Button purchaseButton;

    [Header("Shop Mode")]
    public bool isShopMode = false;

    [HideInInspector] public int outfitIndex = 0;
    [HideInInspector] public int hairIndex = 0;

    private bool updatingFromSpawner = false;

    public bool IsInitialized => isInitialized;


    // When a window becomes active, reload the latest saved customizations from PlayerPrefs
    private void OnEnable() {
        int savedHair = PlayerPrefs.GetInt("HairIndex", 0);
        int savedOutfit = PlayerPrefs.GetInt("OutfitIndex", 0);
        ApplyCustomizationFromSpawner(savedHair, savedOutfit, false);
    }

    // LOAD UNLOCKS
    private void LoadUnlocks() {
        unlockedOutfits = new bool[outfitOptions.Length];
        unlockedHairs = new bool[hairOptions.Length];

        // Load outfit unlock states from PlayerPrefs
        for (int i = 0; i < unlockedOutfits.Length; i++) {
            // Check if outfit at index is default unlocked
            bool isDefault = System.Array.IndexOf(defaultUnlockedOutfits, i) >= 0;
            unlockedOutfits[i] = PlayerPrefs.GetInt("OutfitUnlocked_" + i, isDefault ? 1 : 0) == 1;
        }
        // Load hair unlock states from PlayerPrefs
        for (int i = 0; i < unlockedHairs.Length; i++) {
            // Check if hair at index is default unlocked
            bool isDefault = System.Array.IndexOf(defaultUnlockedHairs, i) >= 0;
            unlockedHairs[i] = PlayerPrefs.GetInt("HairUnlocked_" + i, isDefault ? 1 : 0) == 1;
        }
    }

    // OUTFIT NAVIGATION
    public void NextOutfit() {
        int startIndex = outfitIndex;
        do {
            outfitIndex = (outfitIndex + 1) % outfitOptions.Length; // loop outfits
        } while (!isShopMode && !unlockedOutfits[outfitIndex] && outfitIndex != startIndex); // skip locked outfits unless in shop mode
        ApplyCustomization();
    }

    public void PrevOutfit() {
        int startIndex = outfitIndex;
        do {
            outfitIndex = (outfitIndex - 1 + outfitOptions.Length) % outfitOptions.Length; // loop outfits
        } while (!isShopMode && !unlockedOutfits[outfitIndex] && outfitIndex != startIndex); // skip locked outfits unless in shop mode
        ApplyCustomization();
    }

    // HAIR NAVIGATION
    public void NextHair() {
        int startIndex = hairIndex;
        do {
            hairIndex = (hairIndex + 1) % hairOptions.Length; // loop hairs
        } while (!isShopMode && !unlockedHairs[hairIndex] && hairIndex != startIndex); // skip locked hairs unless in shop mode
        ApplyCustomization();
    }

    public void PrevHair() {
        int startIndex = hairIndex;
        do {
            hairIndex = (hairIndex - 1 + hairOptions.Length) % hairOptions.Length; // loop hairs
        } while (!isShopMode && !unlockedHairs[hairIndex] && hairIndex != startIndex); // skip locked hairs unless in shop mode
        ApplyCustomization();
    }

    // UPDATE TITLES
    // Outfit title
    private void UpdateOutfitTitle() {
        if (outfitTitleText != null) {
            outfitTitleText.text = "outfit " + (outfitIndex + 1);
        }
        if (shopOutfitTitleText != null && isShopMode) {
            shopOutfitTitleText.text = "outfit " + (outfitIndex + 1);
        }
    }

    // Hair title
    private void UpdateHairTitle() {
        if (hairTitleText != null) {
            hairTitleText.text = "hair " + (hairIndex + 1);
        }
        if (shopHairTitleText != null && isShopMode) {
            shopHairTitleText.text = "hair " + (hairIndex + 1);
        }
    }

    // Donation display
    public void UpdateDonationDisplay() {
        if (donationTotalText != null && isShopMode) {
            int currentDonations = PlayerPrefs.GetInt("Donations", 0);
            donationTotalText.text = "$" + currentDonations;
        }
    }

    // APPLY CUSTOMIZATION
    public void ApplyCustomization() {
        if (outfitOptions.Length > 0 && outfitImage != null) {
            outfitImage.sprite = outfitOptions[outfitIndex];
        }
        if (hairOptions.Length > 0 && hairImage != null) {
            hairImage.sprite = hairOptions[hairIndex];
        }

        UpdateOutfitTitle();
        UpdateHairTitle();

        // For shop window preview
        // Determine whether hair or outfit is unlocked/locked
        if (isShopMode && purchaseButton != null) {
            bool outfitLocked = !unlockedOutfits[outfitIndex];
            bool hairLocked = !unlockedHairs[hairIndex];
            purchaseButton.gameObject.SetActive(outfitLocked || hairLocked);

            // Update prices
            // Show price if locked, show "Unlocked" if unlocked
            if (outfitPriceText != null) {
                outfitPriceText.text = outfitLocked ? "$" + outfitPrices[outfitIndex].ToString() : "Unlocked";
            }
            if (hairPriceText != null) {
                hairPriceText.text = hairLocked ? "$" + hairPrices[hairIndex].ToString() : "Unlocked";
            }

            // Update total cost
            if (totalCostText != null) {
                int totalCost = 0;
                if (outfitLocked) totalCost += outfitPrices[outfitIndex];
                if (hairLocked) totalCost += hairPrices[hairIndex];

                totalCostText.text = totalCost > 0 ? "$" + totalCost.ToString() : "$0";
            }

            UpdateDonationDisplay();
        }

        SavePreferences();
        
        if (!updatingFromSpawner) {
            // UPDATE ALL INSTANCES
            AvatarSpawner[] spawners = FindObjectsOfType<AvatarSpawner>();
            foreach (AvatarSpawner spawner in spawners) {
                if (spawner.avatarInstance == null) continue;
                if (spawner.gameObject == this.gameObject) continue; // skip
                
                spawner.UpdateAvatarFromSpawner(hairIndex, outfitIndex);
            }
        }
    }

    // APPLY CUSTOMIZATION FROM SPAWNER
    public void ApplyCustomizationFromSpawner(int hair, int outfit, bool isPreview = false) {
        updatingFromSpawner = true;
        hairIndex = hair;
        outfitIndex = outfit;
        isShopMode = isPreview; // toggle shop mode
        ApplyCustomization();
        updatingFromSpawner = false;
    }

    // PURCHASE
    public void PurchaseCurrentSelection() {
        int currentDonations = PlayerPrefs.GetInt("Donations", 0);
        int totalCost = 0;

        // Add price to total cost
        if (!unlockedOutfits[outfitIndex]) {
            totalCost += outfitPrices[outfitIndex];
        }
        if (!unlockedHairs[hairIndex]) {
            totalCost += hairPrices[hairIndex];
        }
        
        // Unlock only if player has enough donations
        if (currentDonations >= totalCost) {
            if (!unlockedOutfits[outfitIndex]) {
                unlockedOutfits[outfitIndex] = true;
                PlayerPrefs.SetInt("OutfitUnlocked_" + outfitIndex, 1);
            }
            if (!unlockedHairs[hairIndex]) {
                unlockedHairs[hairIndex] = true;
                PlayerPrefs.SetInt("HairUnlocked_" + hairIndex, 1);
            }

            // Update player's total donations
            PlayerPrefs.SetInt("Donations", currentDonations - totalCost);
            PlayerPrefs.Save();
            ApplyCustomization();
        }
        else {
            Debug.Log("Not enough donations");
        }

        UpdateDonationDisplay();
    }

    // SAVE PLAYER PREFERENCES
    private void SavePreferences() {
        PlayerPrefs.SetInt("HairIndex", hairIndex);
        PlayerPrefs.SetInt("OutfitIndex", outfitIndex);
        PlayerPrefs.Save();
    }

    // INITIALZIE
    private bool isInitialized = false;
    public void Initialize() {
        if (isInitialized) return;

        LoadUnlocks();
        hairIndex = PlayerPrefs.GetInt("HairIndex", 0);
        outfitIndex = PlayerPrefs.GetInt("OutfitIndex", 0);

        ApplyCustomization();
        UpdateDonationDisplay();

        isInitialized = true;
    }

    // RESETS
    [ContextMenu("Reset PlayerPrefs Non-Default Unlocks and Avatar")]
    public void ResetAllUnlocksAndAvatar() {
        // Reset all non-default unlocks
        for (int i = 0; i < outfitOptions.Length; i++) {
            bool isDefault = System.Array.IndexOf(defaultUnlockedOutfits, i) >= 0;
            PlayerPrefs.SetInt("OutfitUnlocked_" + i, isDefault ? 1 : 0);
        }
        for (int i = 0; i < hairOptions.Length; i++) {
            bool isDefault = System.Array.IndexOf(defaultUnlockedHairs, i) >= 0;
            PlayerPrefs.SetInt("HairUnlocked_" + i, isDefault ? 1 : 0);
        }

        // Reset avatar
        PlayerPrefs.SetInt("OutfitIndex", 0);
        PlayerPrefs.SetInt("HairIndex", 0);
        PlayerPrefs.Save();

        hairIndex = 0;
        outfitIndex = 0;

        LoadUnlocks();
        ApplyCustomization();

        Debug.Log("PlayerPrefs reset: All outfit and hair unlocks");
    }
}
