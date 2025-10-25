using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
    // This method can be called from a Button's OnClick event
    public void LoadMainScene()
    {
        SceneManager.LoadScene("Loading");
    }
}
