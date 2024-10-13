using SAS.Utilities.TagSystem;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthPresenter : NetworkBehaviour
{
    [FieldRequiresSelf] private Image _healthBar;
    [FieldRequiresParent] private Health _health;

    void Awake()
    {
        this.Initialize();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        _health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, _health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        _health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        _healthBar.fillAmount = (float)newHealth / _health.MaxHealth;
    }
}
