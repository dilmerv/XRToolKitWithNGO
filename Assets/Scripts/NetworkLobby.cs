using DilmerGames.Core.Singletons;
using Unity.Netcode;

public class NetworkLobby : Singleton<NetworkLobby>
{
    private NetworkVariable<int> collaboratorsConnected = new NetworkVariable<int>();

    public NetworkVariable<int> CollaboratorsConnected
    {
        get
        {
            return collaboratorsConnected;
        }
    }

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            collaboratorsConnected.Value++;
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            collaboratorsConnected.Value--;
        };
    }
}
