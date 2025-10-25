using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MessageAlertSystem : MonoBehaviour
{
    public static MessageAlertSystem Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The parent container for the messages (should have a Vertical Layout Group)")]
    public RectTransform messageParent;

    [Tooltip("The prefab containing a TMP_Text and a CanvasGroup")]
    public GameObject messagePrefab;

    [Header("Settings")]
    [Tooltip("How long a message stays visible before fading (seconds)")]
    public float messageDuration = 2f;

    [Tooltip("How long it takes to fade out (seconds)")]
    public float fadeDuration = 0.5f;

    private readonly List<GameObject> activeMessages = new List<GameObject>();

    private void Awake()
    {
        // Singleton pattern for easy global access
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Call this from anywhere: MessageAlertSystem.Instance.ShowMessage("Hello!", Color.green);
    /// </summary>
    public void ShowMessage(string text, Color color)
    {
        if (messagePrefab == null || messageParent == null)
        {
            Debug.LogWarning("MessageAlertSystem is missing its prefab or parent!");
            return;
        }

        // Instantiate the message prefab
        GameObject newMessage = Instantiate(messagePrefab, messageParent);
        newMessage.transform.SetAsFirstSibling(); // Push older ones upward

        // Get the TMP_Text component and CanvasGroup
        TMP_Text messageText = newMessage.GetComponentInChildren<TMP_Text>();
        CanvasGroup canvasGroup = newMessage.GetComponent<CanvasGroup>();

        if (messageText != null)
        {
            messageText.text = text;
            messageText.color = color;
        }
        else
        {
            Debug.LogWarning("Message prefab has no TMP_Text component!");
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        activeMessages.Add(newMessage);

        // Start fade-out coroutine
        StartCoroutine(FadeAndRemove(newMessage, canvasGroup));
    }

    private IEnumerator FadeAndRemove(GameObject message, CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(messageDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        activeMessages.Remove(message);
        Destroy(message);
    }
}
