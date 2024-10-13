using SAS.Pool;
using SAS.Utilities;
using SAS.Utilities.TagSystem;
using System.Collections;
using UnityEngine;

public class Lifetime : MonoBehaviour, ISpawnable
{
    [SerializeField] private float m_Lifetime = 1f;
    [FieldRequiresSelf] private Poolable _poolable;

    private Coroutine _coroutine;

    void Awake()
    {
        this.Initialize();
    }

    void ISpawnable.OnSpawn(object data)
    {
        _coroutine = StaticCoroutine.Start(DespawnObject());
    }

    void ISpawnable.OnDespawn()
    {
        if (_coroutine != null)
        {
            StaticCoroutine.Stop(_coroutine);
            _coroutine = null;
        }
    }

    IEnumerator DespawnObject()
    {
        yield return new WaitForSeconds(m_Lifetime);
        _poolable.Despawn();
    }
}
