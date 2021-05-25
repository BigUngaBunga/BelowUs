using Mirror;

namespace BelowUs
{
    public class ResourceTimeDepletion : NetworkBehaviour
    {
        [SyncVar] protected ShipResource shipResource;
        protected float decreaseFrequency;
        protected float increase;

        protected virtual void Start()
        {
            if (isServer)
            {
                increase = 10;
                shipResource = GetComponent<ShipResource>();
                decreaseFrequency = 0.1f;
                InvokeRepeating(nameof(DecreaseTime), 0, decreaseFrequency);
            }
        }

        [Server] protected virtual void DecreaseTime() => shipResource.ApplyChange(-decreaseFrequency);

        public void IncreaseProductionTime()
        {
            if (isServer)
                shipResource.ApplyChange(increase);
            else
                shipResource.CmdChangeByValue(increase);
        }
    }
}

