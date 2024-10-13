using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool _isDead = false;
    public Action OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    public void RestoreHealth(int health)
    {
        ModifyHealth(health);
    }

    private void ModifyHealth(int value)
    {
        if (_isDead)
            return;

        int health = Mathf.Clamp(CurrentHealth.Value + value, 0, MaxHealth);
        CurrentHealth.Value = health;

        if (health == 0)
        {
            OnDie?.Invoke();
            _isDead = true;
        }
    }

}
