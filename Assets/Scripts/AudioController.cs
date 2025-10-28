using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip typingClip;
    public AudioClip buttonClickClip;
    public AudioClip correctClip;
    public AudioClip incorrectClip;
    public AudioClip resultsPopupClip;
    public AudioClip countdownTickClip;

    // Play sound when a key is typed
    public void PlayTyping() {
        if (typingClip != null && audioSource != null) {
            audioSource.PlayOneShot(typingClip);
        }
    }

    // Play sound when stream input is correct
    public void PlayCorrect() {
        if (correctClip != null && audioSource != null) {
            audioSource.PlayOneShot(correctClip);
        }
    }

    // Play sound when input is incorrect
    public void PlayIncorrect() {
        if (incorrectClip != null && audioSource != null) {
            audioSource.PlayOneShot(incorrectClip);
        }
    }

    // Play sound for general UI button clicks
    public void PlayButtonClip() {
        if (buttonClickClip != null && audioSource != null) {
            audioSource.PlayOneShot(buttonClickClip);
        }
    }

    // Play sound when results window pops up
    public void PlayResultsClip() {
        if (resultsPopupClip != null && audioSource != null) {
            audioSource.PlayOneShot(resultsPopupClip);
        }
    }

    // Play sound after each countdown tick
    public void PlayCountdownTick() {
        if (countdownTickClip != null && audioSource != null) {
            audioSource.PlayOneShot(countdownTickClip);
        }
    }
}
