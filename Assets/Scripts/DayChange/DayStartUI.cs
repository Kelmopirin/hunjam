using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DayStartUI : MonoBehaviour
{
    public CanvasGroup fadeCanvas;       // Full-screen black panel
    public TextMeshProUGUI dayText;      // "Day X" text
    public float fadeDuration = 1.5f;
    public float holdTime = 1.5f;

    void Start()
    {
        ShowDayIntro();
    }

    public void ShowDayIntro()
    {
        int dayNumber = 1;

        if (DayManager.Instance != null)
            dayNumber = DayManager.Instance.GetCurrentDay();

        StartCoroutine(DayIntroSequence(dayNumber));
    }

    private IEnumerator DayIntroSequence(int day)
    {
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 1f;
        dayText.text = $"Day {day}";
        dayText.alpha = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            dayText.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeCanvas.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        fadeCanvas.gameObject.SetActive(false);
    }
}
