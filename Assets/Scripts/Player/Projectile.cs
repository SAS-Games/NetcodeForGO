using SAS.Pool;
using SAS.Utilities.TagSystem;
using System;
using UnityEngine;

public class Projectile : Poolable, ISpawnable
{
    [SerializeField] private float m_Speed;
    [FieldRequiresSelf] private Rigidbody2D _rigidbody;
    [FieldRequiresSelf] private Collider2D _collider;

    protected override void Awake()
    {
        base.Awake();
        this.Initialize();
    }

    void ISpawnable.OnSpawn(object data)
    {
        Tuple<Vector2, Vector2> tuple = (Tuple<Vector2, Vector2>)data;
        var position = tuple.Item1;
        var rotation = tuple.Item2;
        transform.position = position;
        transform.up = rotation;
        if (_rigidbody != null)
            _rigidbody.velocity = _rigidbody.transform.up * m_Speed;
    }

    public void IgnoreCollision(Collider2D other)
    {
        Physics2D.IgnoreCollision(other, _collider);
    }


    void ISpawnable.OnDespawn()
    {
    }
}
