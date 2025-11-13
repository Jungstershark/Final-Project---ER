using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Settings")]
    public GameObject brokenVersion; // Optional: prefab of the broken object
    public bool destroyInstead = false;

    public void Break()
    {
        if (brokenVersion)
            Instantiate(brokenVersion, transform.position, transform.rotation);

        if (destroyInstead || !brokenVersion)
            Destroy(gameObject);

        Debug.Log(gameObject.name + " was broken!");
    }
}
