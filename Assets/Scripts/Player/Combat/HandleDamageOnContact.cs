using Unity.Netcode;
using UnityEngine;

public class HandleDamageOnContact : MonoBehaviour
{
    [SerializeField] private int m_Damage = 5;

    private ulong _ownerClientId;
    public void SetOwner(ulong owner)
    {
        _ownerClientId = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null)
            return;

        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out var networkObject))
        {
            if (_ownerClientId == networkObject.OwnerClientId)
                return;
        }

        if (collision.attachedRigidbody.TryGetComponent<Health>(out var health))
            health.TakeDamage(m_Damage);
    }
}
