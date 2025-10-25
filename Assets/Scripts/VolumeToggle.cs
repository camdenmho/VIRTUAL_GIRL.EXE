using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeToggle : MonoBehaviour
{
    public Button volumeButton;
    public Sprite unmutedSprite;
    public Sprite mutedSprite;
    private Image buttonImage;
    private bool isMuted = false;

    
    void Start()
    {
        buttonImage = volumeButton.GetComponent<Image>();
        buttonImage.sprite = unmutedSprite; // start unmuted

        // Click listener
        volumeButton.onClick.AddListener(ToggleVolume);
    }

    void ToggleVolume() {
        isMuted = !isMuted;

        if (isMuted) {
            buttonImage.sprite = mutedSprite;
            AudioListener.volume = 0f;
        }
        else {
            buttonImage.sprite = unmutedSprite;
            AudioListener.volume = 1f;
        }
    }
}
