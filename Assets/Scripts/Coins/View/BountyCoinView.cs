using SAS.Collectables;
using UnityEngine;

public class BountyCoinView : NetworkCollectible
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    public int Value { get; set; }

    public override void Show(bool show)
    {
        m_SpriteRenderer.enabled = show;
    }
}
