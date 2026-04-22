using UnityEngine;

public class NextLevelLoader : MonoBehaviour
{
    [SerializeField] private string nextLevelSceneName; // Name of the next level scene to load

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load the next level scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelSceneName);
        }
    }

}
