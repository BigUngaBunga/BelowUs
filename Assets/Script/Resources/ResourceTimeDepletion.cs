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
        [SerializeField] private bool emptyResource = false;

        protected virtual void Start()
        {
            if (isServer)
            {
                increase = 10;
                shipResource = GetComponent<ShipResource>();
                repeatTime = 0.1f;
                InvokeRepeating(nameof(DecreaseTime), 0, repeatTime);
            }
        }

        protected virtual void DecreaseTime()
        {

            if (shipResource.CurrentValue <= repeatTime || emptyResource)
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

