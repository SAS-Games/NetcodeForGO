using SAS.Collectables;
using UnityEngine;

public class RespawningCoinView : NetworkCollectible
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private Vector3 _previousPosition;

    private void Update()
    {
        if (_previousPosition != transform.position)
            Show(true);

        _previousPosition = transform.position;
    }

    public override void Show(bool show)
    {
        m_SpriteRenderer.enabled = show;
    }
}
