using UnityEngine;

public class KeyCardSpawner : MonoBehaviour
{
    public EnemyMover enemyMover; // Reference to the EnemyMover component on the same GameObject
    public GameObject keyCardPrefab; // Reference to the KeyCard prefab to be spawned
    public Transform targetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyMover.OnDeath.AddListener(SpawnKeyCard); // Add the SpawnKeyCard method as a listener to the OnDeath event of the EnemyMover component
    }

    private void OnDisable()
    {
        enemyMover.OnDeath.RemoveListener(SpawnKeyCard); // Remove the SpawnKeyCard method as a listener from the OnDeath event when the object is disabled to prevent memory leaks
    }

    private void SpawnKeyCard()
    {
        if (keyCardPrefab != null)
        {
            Instantiate(keyCardPrefab, transform.position, Quaternion.identity).GetComponent<KeyCardMover>().targetPos = targetPos; // Instantiate the KeyCard prefab at the position of the enemy with no rotation
        }
        else
        {
            Debug.LogError("KeyCard prefab is not assigned."); // Log an error if the KeyCard prefab is not assigned
        }

    }


}
