using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSpawner : MonoBehaviour
{
    public GameObject avatarPrefab;
    [HideInInspector] public GameObject avatarInstance;


    void Start()
    {
        // Spawn avatar
        if (avatarPrefab != null) {
            avatarInstance = Instantiate(avatarPrefab, transform);
            avatarInstance.transform.localPosition = Vector3.zero;
            avatarInstance.transform.localScale = Vector3.one;
        }
    }

    public void UpdateAvatarFromSpawner(int hairIndex, int outfitIndex) {
        if (avatarInstance == null) return;

        CharacterCustomization customization = avatarInstance.GetComponent<CharacterCustomization>();
        if (customization != null) {
            customization.ApplyCustomizationFromSpawner(hairIndex, outfitIndex);
        }
    }
}
