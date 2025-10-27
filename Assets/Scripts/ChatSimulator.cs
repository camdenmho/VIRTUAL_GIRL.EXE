using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatSimulator : MonoBehaviour
{
    [Header("Chat Settings")]
    public TextAsset chatMessagesFile;
    public RectTransform chatContent;
    public TMP_Text chatMessagePrefab;
    public float minMessageInterval = 0.5f; // minimum delay between messages
    public float maxMessageInterval = 2f; // maximum delay between messages
    public int maxMessagesOnScreen = 20; // limit visible lines

    private List<string> allMessages = new List<string>();
    private Queue<TMP_Text> activeMessages = new Queue<TMP_Text>();
    private Coroutine chatRoutine;

    void Start()
    {
        LoadChatMessages();
    }

    // Load chat messages from text file
    void LoadChatMessages() {
        allMessages.Clear();

        if (chatMessagesFile != null) {
            string[] lines = chatMessagesFile.text.Split("\n");
            foreach (string line in lines) {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed)) {
                    allMessages.Add(trimmed);
                }
            }
        }
        else {
            Debug.LogWarning("Chat messages file missing");
        }
    }

    // Start chat
    public void StartChat() {
        if (chatRoutine != null) {
            StopCoroutine(chatRoutine);
        }
        chatRoutine = StartCoroutine(ChatRoutine());
    }

    // Stop chat when stream ends
    public void StopChat() {
        if (chatRoutine != null) {
            StopCoroutine(chatRoutine);
            chatRoutine = null;
        }
    }

    // Clear chat
    public void ClearChat() {
        foreach (TMP_Text msg in activeMessages) {
            Destroy(msg.gameObject);
        }
        activeMessages.Clear();
    }

    private IEnumerator ChatRoutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minMessageInterval, maxMessageInterval));

            if (allMessages.Count == 0) continue;

            string newMessage = allMessages[Random.Range(0, allMessages.Count)];

            // Instantiate a new message prefab
            TMP_Text tmp = Instantiate(chatMessagePrefab);
            if (tmp != null) {
                tmp.text = newMessage;
            }
            tmp.transform.SetParent(chatContent, false);

            // Add message to queue
            activeMessages.Enqueue(tmp);

            // Remove oldest message if over max
            while (activeMessages.Count > maxMessagesOnScreen) {
                TMP_Text oldMsg = activeMessages.Dequeue();
                Destroy(oldMsg.gameObject);
            }

            // Scroll to bottom
            ScrollRect sr = chatContent.GetComponentInParent<ScrollRect>();
            if (sr != null) {
                sr.verticalNormalizedPosition = 0f; // scroll to bottom
            }
        }
    }
}
