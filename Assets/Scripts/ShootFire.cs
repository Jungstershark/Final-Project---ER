using UnityEngine;

public class ShootFire : MonoBehaviour
{
    [Header("Shoot Settings")]
    public float range = 100f;
    public float shootCooldown = 0.3f;
    public float fireSpeed = 10f;     // 🔥 how fast projectile moves

    [Header("VFX (Optional)")]
    public GameObject fireEffectPrefab;   // Fireball visual prefab
    public Transform shootPoint;          // Where the fireball spawns (e.g. palm or finger tip)

    private float lastShootTime;

    void Update()
    {
        // Fire when either trigger pressed
        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || 
             OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) &&
            Time.time >= lastShootTime + shootCooldown)
        {
            Debug.Log("Trigger pressed — shooting!");
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        if (!fireEffectPrefab || !shootPoint)
        {
            Debug.LogWarning("⚠️ Missing fireEffectPrefab or shootPoint!");
            return;
        }

        GameObject fireVFX = Instantiate(fireEffectPrefab, shootPoint.position, shootPoint.rotation);
        Debug.Log("🔥 Spawned fireball at " + shootPoint.position);

        // 👉 Manually start any particle system(s) since Play On Awake is off
        var ps = fireVFX.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
        else
        {
            // In case your prefab has multiple child particle systems
            var childSystems = fireVFX.GetComponentsInChildren<ParticleSystem>();
            foreach (var child in childSystems) child.Play();
        }

        // Give it forward velocity if there’s a Rigidbody
        Rigidbody rb = fireVFX.GetComponent<Rigidbody>();
        if (rb)
            rb.linearVelocity = shootPoint.forward * fireSpeed;

        Destroy(fireVFX, 3f);

        // Raycast for immediate hit detection
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Breakable breakable = hit.collider.GetComponent<Breakable>();
            if (breakable)
                breakable.Break();
        }

        // Debug: visualize shooting direction
        Debug.DrawRay(shootPoint.position, shootPoint.forward * 5f, Color.red, 2f);
    }
}
