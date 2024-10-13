using SAS.Pool;
using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] private ComponentPoolSO<Poolable> m_ServerProjectilePool;
    [SerializeField] private ComponentPoolSO<Poolable> m_ClientProjectilePool;
    [SerializeField] private Transform m_SpawnPonit;
    [SerializeField] private InputReader m_InputReader;
    [SerializeField] private Collider2D m_PlayerCollider;
    [SerializeField] private GameObject m_MuzzleFlash;
    [SerializeField, Range(0.01f, 0.1f)] private float m_MuzzleFlashDuration;
    [SerializeField, Range(1f, 10f)] private float m_FireRate;
    private float _fireTime;

    private bool _shouldFire;
    private float _muzzleFlashTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_ClientProjectilePool.Initialize(8);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        m_ServerProjectilePool.Initialize(8);
        m_InputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool fire)
    {
        _shouldFire = fire;
    }

    // Update is called once per frame
    void Update()
    {
        if (_muzzleFlashTimer > 0)
        {
            _muzzleFlashTimer -= Time.deltaTime;
            if (_muzzleFlashTimer < 0)
            {
                _muzzleFlashTimer = 0;
                m_MuzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner)
            return;

        _fireTime -= Time.deltaTime;

        if (!_shouldFire)
            return;

        if (_fireTime > 0)
            return;

        PrimaryFireServerRpc(m_SpawnPonit.position, m_SpawnPonit.up);
        SpawnDummyProjectile(m_SpawnPonit.position, m_SpawnPonit.up);

        _fireTime = 1 / m_FireRate;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        m_InputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector2 spawnPos, Vector2 direction)
    {
        if (IsOwner)
            return;
        SpawnDummyProjectile(spawnPos, direction);

    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector2 spawnPos, Vector2 direction)
    {
        var projectile = m_ServerProjectilePool.Spawn(Tuple.Create<Vector2, Vector2>(spawnPos, direction)) as Projectile;
        projectile.IgnoreCollision(m_PlayerCollider);

        if (projectile.TryGetComponent<HandleDamageOnContact>(out var networkObject))
            networkObject.SetOwner(OwnerClientId);

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector2 spawnPos, Vector2 direction)
    {
        m_MuzzleFlash.SetActive(true);
        _muzzleFlashTimer = m_MuzzleFlashDuration;
        var projectile = m_ClientProjectilePool.Spawn(Tuple.Create<Vector2, Vector2>(spawnPos, direction)) as Projectile;
        projectile.IgnoreCollision(m_PlayerCollider);
    }
}
