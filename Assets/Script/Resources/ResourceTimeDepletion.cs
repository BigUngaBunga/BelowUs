using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class ResourceTimeDepletion : NetworkBehaviour
    {
        [SyncVar] protected ShipResource shipResource;
        [SerializeField] protected float decreaseFrequency;
        [SerializeField] protected float decreaseAmount;
        [SerializeField] protected float increase;

        protected virtual void Start()
        {
            if (isServer)
            {
                shipResource = GetComponent<ShipResource>();
                InvokeRepeating(nameof(DecreaseTime), 0, decreaseFrequency);
            }
        }

        [Server] protected virtual void DecreaseTime() => shipResource.ApplyChange(-decreaseAmount);

        public void IncreaseProductionTime()
        {
            if (isServer)
                shipResource.ApplyChange(increase);
            else
                shipResource.CmdChangeByValue(increase);
        }
    }
}

