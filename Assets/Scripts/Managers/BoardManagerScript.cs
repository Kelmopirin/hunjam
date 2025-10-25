using UnityEngine;
using TMPro;

public class BoardManagerScript : MonoBehaviour
{
    // Singleton instance
    public static BoardManagerScript Instance { get; private set; }

    public TMP_Text textMesh; // your TextMeshPro component

    private void Awake()
    {
        
        Instance = this;
    }

    public void setText(string str)
    {
        if (textMesh != null)
            textMesh.text = str;
    }

    void Start()
    {
    }
}
