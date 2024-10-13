using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoinView m_CoinPrefab;

    [SerializeField] private int m_MaxCoins = 50;
    [SerializeField] private Vector2 m_XSpawnRange;
    [SerializeField] private Vector2 m_YSpawnRange;
    [SerializeField] private LayerMask m_LayerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;
    private EventBinding<CoinCollectedEvent> _coinCollectedEventBinding;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        _coinCollectedEventBinding = new EventBinding<CoinCollectedEvent>(HandleCoinCollected);
        EventBus<CoinCollectedEvent>.Register(_coinCollectedEventBinding);

        coinRadius = m_CoinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < m_MaxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        var coinInstance = Instantiate(
            m_CoinPrefab,
            GetSpawnPoint(),
            Quaternion.identity);

        coinInstance.GetComponent<NetworkObject>().Spawn();
    }

    private void HandleCoinCollected(CoinCollectedEvent coinCollectedEvent)
    {
        var coinView = coinCollectedEvent.coinView;
        coinView.transform.position = GetSpawnPoint();
        coinView.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(m_XSpawnRange.x, m_XSpawnRange.y);
            y = Random.Range(m_YSpawnRange.x, m_YSpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, m_LayerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        EventBus<CoinCollectedEvent>.Deregister(_coinCollectedEventBinding);
    }
}
