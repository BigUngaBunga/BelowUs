using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class OxygenHandler : ResourceTimeDepletion
    {
        [SerializeField] private float consumptionReduction;
        [SerializeReference] [SyncVar] private ShipResource oxygenSeconds;

        [Server]
        protected override void DecreaseTime()
        {
            oxygenSeconds.ApplyChange(-GetConsumption());
            base.DecreaseTime();
        }

        private float GetConsumption() => shipResource.CurrentValue > 0 ? decreaseAmount / consumptionReduction : decreaseAmount;
    }
}