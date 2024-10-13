using SAS.Utilities;
using System.Threading.Tasks;

public class Client : AutoInstantiateSingleton<Client>
{
    public ClientGameManager GameManager { get; private set; }


    public async Task<bool> Create()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager?.Dispose();

    }
}
