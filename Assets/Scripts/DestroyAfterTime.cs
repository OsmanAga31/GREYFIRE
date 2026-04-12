using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float delay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DoDestroy());
    }

    private IEnumerator DoDestroy()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}
