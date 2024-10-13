using SAS.Pool;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour, ISpawnable
{
    [FieldRequiresSelf] private Poolable _poolable;

    void Awake()
    {
        this.Initialize();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _poolable.Despawn();
    }

    void ISpawnable.OnSpawn(object data)
    {
    }

    void ISpawnable.OnDespawn()
    {
    }
}
