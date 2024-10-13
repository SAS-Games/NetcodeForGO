using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image m_HealPowerBar;

    [Header("Settings")]
    [SerializeField] private int m_MaxHealPower = 30;
    [SerializeField] private float m_HealCooldown = 60f;
    [SerializeField] private float m_HealTickRate = 1f;
    [SerializeField] private int m_CoinsPerTick = 10;
    [SerializeField] private int m_HealthPerTick = 10;

    private float _remainingCooldown;
    private float _tickTimer;
    private List<IHealable> _healablesInZone = new List<IHealable>();

    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }

        if (IsServer)
            HealPower.Value = m_MaxHealPower;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
            HealPower.OnValueChanged -= HandleHealPowerChanged;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer)
            return;

        if (!col.attachedRigidbody.TryGetComponent<IHealable>(out var player))
            return;

        _healablesInZone.Add(player);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!IsServer) { return; }

        if (!col.attachedRigidbody.TryGetComponent<IHealable>(out var player))
            return;

        _healablesInZone.Remove(player);
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (_remainingCooldown > 0f)
        {
            _remainingCooldown -= Time.deltaTime;

            if (_remainingCooldown <= 0f)
            {
                HealPower.Value = m_MaxHealPower;
            }
            else
                return;
        }

        _tickTimer += Time.deltaTime;
        if (_tickTimer >= 1 / m_HealTickRate)
        {
            foreach (var healable in _healablesInZone)
            {
                if (HealPower.Value == 0)
                    break;

                if (healable.Heal(m_HealthPerTick, m_CoinsPerTick))
                {
                    HealPower.Value -= 1;
                    if (HealPower.Value == 0)
                        _remainingCooldown = m_HealCooldown;

                }
            }

            _tickTimer = _tickTimer % (1 / m_HealTickRate);
        }
    }

    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        m_HealPowerBar.fillAmount = (float)newHealPower / m_MaxHealPower;
    }
}
