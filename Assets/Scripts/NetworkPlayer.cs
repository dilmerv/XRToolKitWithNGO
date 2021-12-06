using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private Vector2 placementArea = new Vector2(5.0f, 5.0f);

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
            transform.position = new Vector3(Random.Range(0.0f, placementArea.x),
                transform.position.y, Random.Range(0.0f, placementArea.y));
        }
    }
}
