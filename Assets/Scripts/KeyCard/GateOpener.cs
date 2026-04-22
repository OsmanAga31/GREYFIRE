using UnityEngine;

public class GateOpener : MonoBehaviour
{
    [SerializeField] private Animator gateAnimator; // Reference to the Animator component controlling the gate
    [SerializeField] private AudioSource gateAudioSource; // Reference to the AudioSource component for playing gate opening sound
    [SerializeField] private AudioClip gateClip; // Reference to the AudioClip for the gate opening sound
    public Transform point1;
    public Transform point2;

    public void OpenGate()
    {
        // Implement the logic to open the gate here
        Debug.Log("Gate is now open!");
        if (gateAnimator != null)
        {
            gateAnimator.SetBool("isGateOpen", true); // Trigger the "Open" animation
            if (gateAudioSource != null && gateClip != null)
            {
                gateAudioSource.PlayOneShot(gateClip); // Play the gate opening sound
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip is not assigned.");
            }

            // Get Material and change emission color to a greenish color
            Renderer gateRenderer = GetComponent<Renderer>();
            if (gateRenderer != null)
            {
                Material gateMaterial = gateRenderer.material;
                gateMaterial.SetColor("_EmissionColor", new Color(0.2f, 1f, 0.2f)); // Set the emission color to a greenish color
            }
            else
            {
                Debug.LogWarning("Renderer component is not found on the gate.");
            }
        }
        else
        {
            Debug.LogError("Gate Animator is not assigned.");
        }
    }
}
