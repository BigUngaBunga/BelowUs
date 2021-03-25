using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // exit from update if this is not the local player
        if (!isLocalPlayer) return;

        // TODO handle player input for movement
    }
}
