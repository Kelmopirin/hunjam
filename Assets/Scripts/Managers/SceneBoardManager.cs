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
            BoardManagerScript.Instance.setText("Főzz valami finomat alvás előtt! Szedj össze mindent az üstbe, utána aludhatsz! Tipp: Egyes tárgyakhoz több funckió is tartozhat!");
        }
        else if (sceneIndex == 2)
        {
            // Logic for scene 2
            Debug.Log("Scene 2 logic here");
            BoardManagerScript.Instance.setText("Holnap AdatB ZH. Alváshoz olvasd el Gajdos könyvét és csinálj kaját! Tipp: Keresd meg a oltárat és a könyvet, olvasd el Gajdos mesterművét ott. Az energiaital feltölti az energiádat.");
        }
        else if (sceneIndex == 3)
        {
            // Logic for scene 3
            Debug.Log("Scene 3 logic here");
            BoardManagerScript.Instance.setText("A tegnapi buli eredménye látható a folyosón! Vigyázz a foltokkal és főzz valami finomat! Ne beszélj a mosdóban a csávóval!");
        }
        else if (sceneIndex == 4)
        {
            // Logic for scene 4
            Debug.Log("Scene 4 logic here");
            BoardManagerScript.Instance.setText("Mintha te is ittál volna. Csak óvatosan, eléggé érdekesen közlekedsz. A folyosón a tegnapi csávó elszabadult. Főzz, sok sikert!");
        }
        else if (sceneIndex == 5)
        {
            // Logic for scene 5
            Debug.Log("Scene 5 logic here");
            BoardManagerScript.Instance.setText("Tartozol a szobatársadnak! Egy doboz cigit szerezned kell az utolsó napon! Pénzt a RePont-ban tudsz szerezni. Sok mindent elfogad a gép! Dohiban vehetsz cigit! Főzz is!");
        }
        else
        {
            // Optional: default logic for other scenes
            Debug.Log("Other scene logic");
        }
    }
}
