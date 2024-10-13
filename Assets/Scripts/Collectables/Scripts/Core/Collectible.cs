using UnityEngine;

namespace SAS.Collectables
{
    public class Collectible : MonoBehaviour, ICollectible
    {
        [SerializeField] private CollectibleSOBase<ICollectible> _collectibleSO;
        public void Collect(ICollector collector)
        {
            _collectibleSO.TryCollect(this, collector);
        }

        public void Reset()
        {
        }
    }
}
