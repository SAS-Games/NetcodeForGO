namespace SAS.Collectables
{
    public interface ICollectible
    {
        void Collect(ICollector collector);
        void Reset();
    }
}
