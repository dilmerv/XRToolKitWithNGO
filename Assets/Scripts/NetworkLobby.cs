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
            if(IsHost || IsServer)
                collaboratorsConnected.Value++;
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            if (IsHost || IsServer)
                collaboratorsConnected.Value--;
        };
    }
}
