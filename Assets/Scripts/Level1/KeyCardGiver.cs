using UnityEngine;

public class KeyCardGiver : MonoBehaviour
{
    [SerializeField] private GameObject keyCardPrefab; // Reference to the KeyCard prefab to be spawned
    [SerializeField] private Transform targetPos; // Reference to the target position for the KeyCardMover


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyMover[] enemies = GetComponentsInChildren<EnemyMover>(); // Get all Enemy components in the children of the parent GameObject
        int randomIndex = Random.Range(0, enemies.Length); // Generate a random index to select an enemy
        EnemyMover selectedEnemy = enemies[randomIndex]; // Get the randomly selected enemy
        Debug.Log("Selected Enemy: " + selectedEnemy.name); // Log the name of the selected enemy for debugging purposes
        KeyCardSpawner KCS = selectedEnemy.gameObject.AddComponent<KeyCardSpawner>(); // Add the KeyCard component to the selected enemy's GameObject
        KCS.enemyMover = selectedEnemy; // Set the enemyMover reference in the KeyCardSpawner to the selected enemy
        KCS.keyCardPrefab = keyCardPrefab; // Set the keyCardPrefab reference in the KeyCardSpawner to the specified prefab
        KCS.targetPos = targetPos;
    }


}
