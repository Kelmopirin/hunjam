using UnityEngine;
using TMPro;

public class BoardManagerScript : MonoBehaviour
{
    public TMPro.TextMeshPro textMesh;
    void setText(string str)
    {
        textMesh.text = str;
    }
    void Start()
    {
        setText("Placeholder");
    }
}
