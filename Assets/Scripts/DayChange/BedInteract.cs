using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class BedInteract: MonoBehaviour
{
    public float interactionRange = 6f;
    public Transform player;
    public GameObject sleepPrompt;

    public Boolean canNextLevel = false;

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
                if (!canNextLevel)
            {
                FindObjectOfType<MessageAlertSystem>().ShowMessage("Még nem aludhatsz!", Color.red);
                return;
            }
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
