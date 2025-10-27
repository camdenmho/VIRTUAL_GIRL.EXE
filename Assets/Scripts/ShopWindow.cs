using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public CharacterCustomization customization;

    private void OnEnable() {
        if (customization != null) {
            customization.isShopMode = true;
            customization.ApplyCustomization(); // Refresh avatar and prices
        }
    }

    private void OnDisable() {
        if (customization != null) {
            customization.isShopMode = false;
            customization.ApplyCustomization(); // Revert to show only unlocked items
        }
    }
}
