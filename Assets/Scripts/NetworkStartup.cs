using Unity.Netcode;
using UnityEngine;

public class NetworkStartup : MonoBehaviour
{
    void Start()
    {
        if (SceneTransitionHandler.Instance.InitializeAsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
