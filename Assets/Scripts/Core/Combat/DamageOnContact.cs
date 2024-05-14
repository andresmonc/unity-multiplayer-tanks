using Unity.Netcode;
using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rigidBody = other.attachedRigidbody;
        if (rigidBody == null)
        {
            Debug.Log("no rigid body!");
            return;
        }
        if (rigidBody.TryGetComponent<Health>(out Health health))
        {
            Debug.Log("has health!");
            health.TakeDamage(damage);
        }
    }
}
