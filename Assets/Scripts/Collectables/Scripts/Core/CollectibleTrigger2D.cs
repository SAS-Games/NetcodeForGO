using UnityEngine;

namespace SAS.Collectables
{
    public class CollectibleTrigger2D : MonoBehaviour
    {
        [SerializeField] private LayerMask m_WhoCanCollect;

        private ICollectible _collectible;
        private void Awake()
        {
            _collectible = GetComponentInParent<ICollectible>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (m_WhoCanCollect.Contains(collision.gameObject.layer))
                _collectible?.Collect(collision.GetComponentInParent<ICollector>());
        }
    }
}
