using SAS.Collectables;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/RespawningCoin")]
public class RespawningCoinSO : CoinBaseSO
{
    [SerializeField] private int m_CoinValue = 10;

    public override bool TryCollect(ICollectible collectible, ICollector collector)
    {
        if (base.TryCollect(collectible, collector))
        {
            var coinView = collectible as RespawningCoinView;
            var coinWallet = collector as CoinWallet;
            if (coinWallet != null)
            {
                coinWallet.TotalCoins.Value += m_CoinValue;
                EventBus<CoinCollectedEvent>.Raise(new CoinCollectedEvent
                {
                    value = m_CoinValue,
                    coinView = coinView
                });
                return true;
            }
        }
        return false;
    }
}