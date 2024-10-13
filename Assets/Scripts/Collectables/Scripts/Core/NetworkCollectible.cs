using Unity.Netcode;
using UnityEngine;

namespace SAS.Collectables
{
    public abstract class NetworkCollectible : NetworkBehaviour, ICollectible
    {
        [SerializeField] private CollectibleSOBase<ICollectible> m_CollectibleSO;
        private bool _alreadyCollected;


        public void Collect(ICollector collector)
        {
            if (!_alreadyCollected && collector != null)
            {
                m_CollectibleSO.TryCollect(this, collector);
                _alreadyCollected = true;
            }
        }

        public void Reset()
        {
            _alreadyCollected = false;
        }
        abstract public void Show(bool show);
    }

}
