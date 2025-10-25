using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    public string gameSceneName = "main"; // Name of the actual level
    public float minimumLoadTime = 5f; // Minimum time to show loading screen

    private void Start()
    {
        StartCoroutine(LoadGameAsync());
    }

    private IEnumerator LoadGameAsync()
    {
        float startTime = Time.time;

        // Start loading the game scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false; // Wait to activate manually

        // Wait until the scene is loaded (progress reaches 0.9)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Wait until at least minimumLoadTime seconds have passed
        float elapsed = Time.time - startTime;
        if (elapsed < minimumLoadTime)
            yield return new WaitForSeconds(minimumLoadTime - elapsed);

        // Activate the game scene
        asyncLoad.allowSceneActivation = true;
    }
}
