using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCustomization : MonoBehaviour
{
    [Header("Avatar Layers")]
    public Image outfitImage;
    public Image hairImage;

    [Header("Options")]
    public Sprite[] outfitOptions;
    public Sprite[] hairOptions;

    [Header("UI Titles")]
    public TMP_Text outfitTitleText;
    public TMP_Text hairTitleText;

    private int outfitIndex = 0;
    private int hairIndex = 0;

    // PLAYERPREFS
    private void Start() {
        hairIndex = PlayerPrefs.GetInt("HairIndex", 0);
        outfitIndex = PlayerPrefs.GetInt("OutfitIndex", 0);
        ApplyCustomization();
    }

    // OUTFIT
    public void NextOutfit() {
        if (outfitOptions.Length == 0) return;
        outfitIndex = (outfitIndex + 1) % outfitOptions.Length;
        ApplyCustomization();
    }

    public void PrevOutfit() {
        if (outfitOptions.Length == 0) return;
        outfitIndex = (outfitIndex - 1 + outfitOptions.Length) % outfitOptions.Length;
        ApplyCustomization();
    }

    // HAIR
    public void NextHair() {
        if (hairOptions.Length == 0) return;
        hairIndex = (hairIndex + 1) % hairOptions.Length;
        ApplyCustomization();
    }

    public void PrevHair() {
        if (hairOptions.Length == 0) return;
        hairIndex = (hairIndex - 1 + hairOptions.Length) % hairOptions.Length;
        ApplyCustomization();
    }

    // UPDATE TITLES
    private void UpdateOutfitTitle() {
        if (outfitTitleText != null) {
            outfitTitleText.text = "outfit " + (outfitIndex + 1);
        }
    }

    private void UpdateHairTitle() {
        if (hairTitleText != null) {
            hairTitleText.text = "hair " + (hairIndex + 1);
        }
    }

    // APPLY CUSTOMIZATION
    private void ApplyCustomization() {
        if (outfitOptions.Length > 0) {
            outfitImage.sprite = outfitOptions[outfitIndex];
        }
        if (hairOptions.Length > 0) {
            hairImage.sprite = hairOptions[hairIndex];
        }

        UpdateOutfitTitle();
        UpdateHairTitle();

        SavePreferences();
    }

    // SAVE PREFERENCES
    private void SavePreferences() {
        PlayerPrefs.SetInt("HairIndex", hairIndex);
        PlayerPrefs.SetInt("OutfitIndex", outfitIndex);
        PlayerPrefs.Save();
    }

    // RESET
    public void ResetCustomization() {
        hairIndex = 0;
        outfitIndex = 0;
        ApplyCustomization();
    }
}
