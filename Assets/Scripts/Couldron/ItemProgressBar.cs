using UnityEngine;
using UnityEngine.UI;

public class ItemProgressBar : MonoBehaviour
{
    public Slider slider;

    public void FillForItem(string itemName)
    {
        Debug.Log($"Filling progress bar for item: {itemName}");
        // your fill logic here
    }
}