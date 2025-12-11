using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage! Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        // Death animation, drop items, etc.
        Destroy(gameObject);
    }
}