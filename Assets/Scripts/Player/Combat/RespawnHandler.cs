using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private Tank m_PlayerPrefab;
    [SerializeField] private float m_KeptCoinPercentage;

    private EventBinding<PlayerSpawnedEvent> _playerSpawnedEventBinding;
    private EventBinding<PlayerDespawnedEvent> _playerDespawnedEventBinding;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        var players = FindObjectsByType<Tank>(FindObjectsSortMode.None);
        foreach (var player in players)
            HandlePlayerSpawned(new PlayerSpawnedEvent { tank = player });

        _playerSpawnedEventBinding = new EventBinding<PlayerSpawnedEvent>(HandlePlayerSpawned);
        EventBus<PlayerSpawnedEvent>.Register(_playerSpawnedEventBinding);

        _playerDespawnedEventBinding = new EventBinding<PlayerDespawnedEvent>(HandlePlayerDespawned);
        EventBus<PlayerDespawnedEvent>.Register(_playerDespawnedEventBinding);

    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        EventBus<PlayerSpawnedEvent>.Deregister(_playerSpawnedEventBinding);
        EventBus<PlayerDespawnedEvent>.Deregister(_playerDespawnedEventBinding);
    }

    private void HandlePlayerSpawned(PlayerSpawnedEvent playarSpawned)
    {
        playarSpawned.tank.Health.OnDie += () => HandlePlayerDie(playarSpawned.tank);
    }

    private void HandlePlayerDespawned(PlayerDespawnedEvent playarDespawned)
    {
        playarDespawned.tank.Health.OnDie -= () => HandlePlayerDie(playarDespawned.tank);

    }

    private void HandlePlayerDie(Tank player)
    {
        int keptCoins = (int)(player.Wallet.TotalCoins.Value * (m_KeptCoinPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
    {
        yield return null;

        var player = Instantiate(
            m_PlayerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        player.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        player.Wallet.TotalCoins.Value += keptCoins;
    }
}
