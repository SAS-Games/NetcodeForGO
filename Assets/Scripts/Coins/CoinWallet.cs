using SAS.Collectables;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour, ICollector
{
    [Header("References")]
    [SerializeField] private Health m_Health;
    [SerializeField] private BountyCoinView m_BountyCoinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;


        coinRadius = m_BountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

        m_Health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        m_Health.OnDie -= HandleDie;
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void HandleDie()
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
            return;

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoinView coinInstance = Instantiate(m_BountyCoinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.NetworkObject.Spawn();
            coinInstance.Value = bountyCoinValue;
        }
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
                return spawnPoint;
        }
    }
}
