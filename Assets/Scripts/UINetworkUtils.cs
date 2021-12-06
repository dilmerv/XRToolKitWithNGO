using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkUtils : MonoBehaviour
{
    [SerializeField]
    private Button startHost;

    [SerializeField]
    private Button startClient;

    // Start is called before the first frame update
    void Start()
    {
        startHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        startClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
