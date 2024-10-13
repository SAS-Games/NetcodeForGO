using SAS.Utilities;
using Unity.Netcode;

public class Host : AutoInstantiateSingleton<Host>
{
    public HostGameManager GameManager { get; private set; }
    public void Create(NetworkObject playerPrefab)
    {
        GameManager = new HostGameManager(playerPrefab);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager?.Dispose();
    }
}
