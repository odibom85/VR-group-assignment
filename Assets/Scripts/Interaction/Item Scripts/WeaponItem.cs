using UnityEngine;

public class WeaponItem : MonoBehaviour, IUsableItem
{
    public string itemName = "Bat";
    public float damage = 10f;
    public float hitRange = 2f;
    public LayerMask hitLayers; // Set what can be hit in inspector

    private CombinedInteraction playerInteraction;

    void Start()
    {
        playerInteraction = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<CombinedInteraction>();
    }

    public void Use()
    {
        Debug.Log("Swinging weapon!");

        // Raycast from player to see what we hit
        RaycastHit? lookTarget = playerInteraction.GetLookTarget();

        if (lookTarget.HasValue)
        {
            RaycastHit hit = lookTarget.Value;

            // Check if hit object can take damage
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log("Hit " + hit.collider.name + " for " + damage + " damage!");
                damageable.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Hit " + hit.collider.name + " but it can't take damage");
            }
        }
        else
        {
            Debug.Log("Swung weapon but didn't hit anything");
        }

        // Optional: Add swing animation, sound effects, etc.
    }

    public string GetItemName()
    {
        return itemName;
    }
}