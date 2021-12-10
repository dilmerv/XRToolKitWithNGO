using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<uint> agoraUserId = new NetworkVariable<uint>();

    [SerializeField]
    private NetworkVariable<bool> overlayOverPlayer = new NetworkVariable<bool>();

    [SerializeField]
    private Vector2 placementArea = new Vector2(-10.0f, 10.0f);

    public override void OnNetworkSpawn() => DisableClientInput();

    private void DisableClientInput()
    {
        if (IsClient && !IsOwner)
        {
            var clientMoveProvider = GetComponent<NetworkMoveProvider>();
            var clientControllers = GetComponentsInChildren<ActionBasedController>();
            var clientTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();
            var clientHead = GetComponentInChildren<TrackedPoseDriver>();
            var clientCamera = GetComponentInChildren<Camera>();

            clientCamera.enabled = false; 
            clientMoveProvider.enableInputActions = false;
            clientTurnProvider.enableTurnAround = false;
            clientTurnProvider.enableTurnLeftRight = false;
            clientHead.enabled = false;

            foreach (var input in clientControllers)
            {
                input.enableInputActions = false;
                input.enableInputTracking = false;
            }
        }
    }

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(placementArea.x, placementArea.y),
                transform.position.y, Random.Range(placementArea.x, placementArea.y));

            AgoraVideoSetup.Instance.StartAgora();
        }
    }

    private void Update()
    {
        if (IsClient && !IsOwner) return;

        if(agoraUserId.Value == 0)
        {
            UpdateAgoraUserIdServerRpc(AgoraVideoSetup.Instance.GetAgoraUserId());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleOverlayPlacementServerRpc();
        }
    }

    private void TogglePlayerOverlay()
    {
        Transform canvasScreen = GameObject.Find("Videos").transform;
        var canvasPlayer = GetComponentInChildren<Canvas>().transform;

        Transform playerVideo = canvasScreen.transform.Find($"{agoraUserId.Value}");

        if (playerVideo != null)
        {
            playerVideo.transform.parent = canvasPlayer;
        }
        else // perhaps is on the player already
        {
            playerVideo = canvasPlayer.transform.Find($"{agoraUserId.Value}");
            playerVideo.transform.parent = canvasScreen;
        }

        playerVideo.localPosition = new Vector3(0, 0, 0);
        playerVideo.localRotation = new Quaternion(0, 0, -180, 0);
    }

    [ServerRpc]
    public void UpdateAgoraUserIdServerRpc(uint newAgoraUserId)
    {
        agoraUserId.Value = newAgoraUserId;
    }

    [ServerRpc]
    public void ToggleOverlayPlacementServerRpc()
    {
        PlaceOverlayOnPlayersClientRpc();
    }

    [ClientRpc]
    public void PlaceOverlayOnPlayersClientRpc()
    {
        TogglePlayerOverlay();
    }

    public void OnSelectGrabbable(SelectEnterEventArgs eventArgs)
    {
        if (IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null)
                RequestGrabbableOwnershipServerRpc(OwnerClientId, networkObjectSelected);
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
    }
}
