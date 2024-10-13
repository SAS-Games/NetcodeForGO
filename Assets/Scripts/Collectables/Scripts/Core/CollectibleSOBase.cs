using UnityEngine;

namespace SAS.Collectables
{
    public abstract class CollectibleSOBase<T> : ScriptableObject where T : ICollectible
    {
        public abstract bool TryCollect(T collectible, ICollector collector);
    }
}
