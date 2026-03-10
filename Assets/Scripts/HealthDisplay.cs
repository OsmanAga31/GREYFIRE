using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText; // Reference to the Text component that displays the health value
    
    public void UpdateHealthDisplay(float currentHealth)
    {
        // Update the health text to show the current health value
        healthText.text = Mathf.RoundToInt(currentHealth).ToString();
    }
}
