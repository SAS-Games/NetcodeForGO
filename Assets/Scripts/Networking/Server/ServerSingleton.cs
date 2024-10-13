using SAS.Utilities;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;

public class ServerSingleton : AutoInstantiateSingleton<ServerSingleton>
{
    public ServerGameManager GameManager { get; private set; }


    public async Task CreateServer(NetworkObject playerPrefab)
    {
        await UnityServices.InitializeAsync();

        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton,
            playerPrefab
        );
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager?.Dispose();
    }
}
