using Unity.Netcode;
using UnityEngine;

public class UINetworkUtils : MonoBehaviour
{
    void Start()
    {
        if (NetworkLobby.Instance.CollaboratorsConnected.Value == 0)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
