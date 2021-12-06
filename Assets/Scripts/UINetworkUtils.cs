using Unity.Netcode;
using UnityEngine;

public class UINetworkUtils : MonoBehaviour
{
    void Start()
    {
        if (SceneTransitionHandler.Instance.StartAsServer)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
