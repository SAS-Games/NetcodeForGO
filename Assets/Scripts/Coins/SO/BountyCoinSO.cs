using SAS.Collectables;
using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/BountyCoin")]

public class BountyCoinSO : CoinBaseSO
{
    public override bool TryCollect(ICollectible collectible, ICollector collector)
    {
        if (base.TryCollect(collectible, collector))
        {
            var coinWallet = collector as CoinWallet;
            if (coinWallet != null)
            {
                var coinView = collectible as BountyCoinView;
                coinWallet.TotalCoins.Value += coinView.Value;
                Destroy(coinView.gameObject);
                return true;
            }
        }
        return false;
    }
}
