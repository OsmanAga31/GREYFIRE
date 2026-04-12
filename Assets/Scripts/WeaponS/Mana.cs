using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public static Mana Instance { get; private set; }

    [SerializeField] private float currentMana;
    public float CurrentMana
    {
        get => currentMana;
    }
    [SerializeField] private float maxMana = 100f; // Maximum mana
    [SerializeField] private float manaRegenRate = 5f; // Mana regeneration rate per second

    [SerializeField] private Image manaDisplay;
    [SerializeField] private float manaDisplayLerpSpeed = 5f; // Speed of the lerp for smooth transition of the mana display
    [SerializeField] private float manaRegenDelay = 1f; // Delay before mana starts regenerating

    private bool canConsumeMana = true; // Flag to control mana consumption
    public bool CanConsumeMana
    {
        get => canConsumeMana;
        set => canConsumeMana = value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        currentMana = maxMana; // Start with full mana
        StartCoroutine(ManaRegen());
    }

    private void Update()
    {
        // Update the UI by lerping for smooth transition
        if (manaDisplay != null)
        {
            var targetFillAmount = currentMana / maxMana;
            manaDisplay.fillAmount = Mathf.Lerp(manaDisplay.fillAmount, targetFillAmount, Time.deltaTime * manaDisplayLerpSpeed);
        }
    }

    private IEnumerator ManaRegen() {         
        while (true)
        {
            yield return new WaitForSeconds(manaRegenDelay);
            currentMana += manaRegenRate;
            currentMana = Mathf.Clamp(currentMana, 0f, maxMana); // Assuming max mana is 100
        }
    }

    public bool ConsumeMana(float amount)
    {
        canConsumeMana = true;
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }
        else
        {
            Debug.Log("Not enough mana!");
            return false;
        }
    }

}
