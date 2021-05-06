using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class ResourceTimeDepletion : NetworkBehaviour
    {
        [SyncVar] protected ShipResource shipResource;
        protected float repeatTime;
        protected float increase;

        protected virtual void Start()
        {
            if (isServer)
            {
                increase = 2;
                shipResource = GetComponent<ShipResource>();
                repeatTime = 0.1f;
                InvokeRepeating(nameof(DecreaseTime), 0, repeatTime);
            }
        }

        protected virtual void DecreaseTime()
        {

            if (shipResource.CurrentValue <= repeatTime)
                shipResource.SetValue(0);
            else
                shipResource.ApplyChange(-repeatTime);
        }

        public void IncreaseProductionTime()
        {
            if (shipResource.CurrentValue + increase > shipResource.maximumValue)
                shipResource.SetValue(shipResource.maximumValue);
            else
                shipResource.ApplyChange(increase);
        }
    }
}

