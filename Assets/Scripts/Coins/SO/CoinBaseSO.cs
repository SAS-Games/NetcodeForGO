using SAS.Collectables;

struct CoinCollectedEvent : IEvent
{
    public int value;
    public RespawningCoinView coinView;
}

public abstract class CoinBaseSO : CollectibleSOBase<ICollectible>
{
    public override bool TryCollect(ICollectible collectible, ICollector collector)
    {
        var coinView = collectible as NetworkCollectible;

        if (!coinView.IsServer)
        {
            coinView.Show(false);
            return false;
        }
        return true;
    }
}
