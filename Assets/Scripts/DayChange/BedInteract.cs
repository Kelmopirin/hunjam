using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BedInteract: MonoBehaviour
{
    public float interactionRange = 2.5f;
    public Transform player;
    public GameObject sleepPrompt;

    private PlayerInput playerInput;
    private bool isInRange = false;

    void Awake()
    {
        if (player != null)
            playerInput = player.GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        isInRange = distance <= interactionRange;

        if (sleepPrompt != null)
            sleepPrompt.SetActive(isInRange);

        if (isInRange && playerInput != null)
        {
            var interactAction = playerInput.actions["Interact"];
            if (interactAction != null && interactAction.triggered)
            {
                GoToNextScene();
            }
        }
    }

    private void GoToNextScene()
    {
        if (DayManager.Instance != null)
            DayManager.Instance.NextDay();
        else
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            int nextScene = currentScene + 1;
            if (nextScene >= SceneManager.sceneCountInBuildSettings)
                nextScene = 0;

            SceneManager.LoadScene(nextScene);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
