using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVersion; // Reference to the destroyed version of the object

    public void Destroy()
    {
        // Instantiate the destroyed version of the object at the same position and rotation
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        // Destroy the original object
        Destroy(gameObject);
    }

}
