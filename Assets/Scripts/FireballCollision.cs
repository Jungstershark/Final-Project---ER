using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Breakable breakable = collision.collider.GetComponentInParent<Breakable>();
        if (breakable)
        {
            breakable.Break();
        }

        // Optional: destroy the fireball on impact
        Destroy(gameObject);
    }
}
