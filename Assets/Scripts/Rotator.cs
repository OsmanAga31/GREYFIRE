using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private bool rotateClockwise = true;
    [SerializeField] private bool rotateInWorldSpace = false;

    // Update is called once per frame
    void Update()
    {
        float direction = rotateClockwise ? 1f : -1f;
        Vector3 rotation = rotationAxis.normalized * rotationSpeed * direction * Time.deltaTime;
        if (rotateInWorldSpace)
        {
            transform.Rotate(rotation, Space.World);
        }
        else
        {
            transform.Rotate(rotation, Space.Self);
        }
    }
}
