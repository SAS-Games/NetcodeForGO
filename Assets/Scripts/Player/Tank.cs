using Cinemachine;
using SAS.Utilities.TagSystem;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

struct PlayerSpawnedEvent : IEvent
{
    public Tank tank;

    public static explicit operator PlayerSpawnedEvent(Tank t)
    {
        return new PlayerSpawnedEvent { tank = t };
    }
}

struct PlayerDespawnedEvent : IEvent
{
    public Tank tank;
}

public class Tank : NetworkBehaviour, IHealable
{
    [SerializeField] private int m_OwnerPiority = 15;
    [Header("MinimapSettings")]
    [SerializeField] private SpriteRenderer m_MinimapIconRenderer;
    [SerializeField] private Color m_OwnerColour;
    [FieldRequiresChild] private CinemachineVirtualCamera _virtualCamera;
    [field: FieldRequiresChild] public Health Health { get; private set; }
    [field: FieldRequiresChild] public CoinWallet Wallet { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> teamIndex = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        this.Initialize();
        if (IsServer)
        {
            UserData userData = null;

            if (IsHost)
                userData = Host.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            else
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            playerName.Value = userData.userName;
            teamIndex.Value = userData.teamIndex;
            EventBus<PlayerSpawnedEvent>.Raise(new PlayerSpawnedEvent { tank = this });

        }
        if (IsOwner)
        {
            _virtualCamera.Priority = m_OwnerPiority;
            m_MinimapIconRenderer.color = m_OwnerColour;
            //Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
        }

    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            EventBus<PlayerDespawnedEvent>.Raise(new PlayerDespawnedEvent { tank = this });
    }

    bool IHealable.Heal(int healthDelta, int cost)
    {
        if (Health.CurrentHealth.Value == Health.MaxHealth)
            return false;

        if (Wallet.TotalCoins.Value < cost)
            return false;

        Wallet.SpendCoins(cost);
        Health.RestoreHealth(healthDelta);
        return true;
    }
}
