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
        var actionbasedControllers = GetComponentsInChildren<ActionBasedController>();


        if (IsClient && !IsOwner)
        {
            moveProvider.DisableInput = true;
            GetComponentInChildren<Camera>().enabled = false;
            foreach(var input in actionbasedControllers)
            {
                input.enableInputActions = false;
                input.enableInputTracking = false;
            }
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
