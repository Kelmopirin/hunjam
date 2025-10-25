using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneBoardManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public BoardManagerScript boardManager; // assign in Inspector

     void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 1)
        {
            // Logic for scene 1
            Debug.Log("Scene 1 logic here");
            BoardManagerScript.Instance.setText("Főzz valami finomat alvás előtt! Szedj össze mindent az üstbe!");
        }
        else if (sceneIndex == 2)
        {
            // Logic for scene 2
            Debug.Log("Scene 2 logic here");
            BoardManagerScript.Instance.setText("Holnap AdatB ZH. Addig nem fogsz tudni aludni amíg meg nem emészted Gajdos szentírását!");
        }
        else if (sceneIndex == 3)
        {
            // Logic for scene 3
            Debug.Log("Scene 3 logic here");
            BoardManagerScript.Instance.setText("Részeg vagy xd");
        }
        else if (sceneIndex == 4)
        {
            // Logic for scene 4
            Debug.Log("Scene 4 logic here");
            BoardManagerScript.Instance.setText("Day 4");
        }
        else if (sceneIndex == 5)
        {
            // Logic for scene 5
            Debug.Log("Scene 5 logic here");
            BoardManagerScript.Instance.setText("Day 5");
        }
        else
        {
            // Optional: default logic for other scenes
            Debug.Log("Other scene logic");
        }
    }
}
