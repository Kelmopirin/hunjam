using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteract : MonoBehaviour
{
    public bool canNextLevel = false;

    public void TrySleep()
    {
        if (!canNextLevel)
        {
            FindObjectOfType<MessageAlertSystem>().ShowMessage("Még nem aludhatsz!", Color.red);
            return;
        }

        GoToNextScene();
    }

    private void GoToNextScene()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.NextDay();
            return;
        }

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene >= SceneManager.sceneCountInBuildSettings)
            nextScene = 0;

        SceneManager.LoadScene(nextScene);
    }
}
