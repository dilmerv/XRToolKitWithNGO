using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        var moveProvider = GetComponent<NetworkMoveProvider>();
        if (IsClient && !IsOwner)
        {
            moveProvider.DisableInput = true;
            GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            moveProvider.DisableInput = false;
        }    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
