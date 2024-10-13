using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUIs : MonoBehaviour
{
    [SerializeField] private Button m_JoinButton;
    [SerializeField] private Button m_HostButton;
    // Start is called before the first frame update
    void Start()
    {
        m_JoinButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });

        m_HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
    }
}
