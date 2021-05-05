using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class OxygenHandler : NetworkBehaviour
    {
        [SerializeReference] private ShipResource oxygenProduction;
        [SerializeField] private float consumptionReduction;
        private ShipResource oxygenSeconds;

        private void Awake() => oxygenSeconds = GetComponentInParent<ShipResource>();

        private void Update() => DecreaseTime(GetConsumption());

        [ClientRpc]
        private void DecreaseTime(float consumption)
        {
            if (isServer)
            {
                oxygenSeconds.ApplyChange(-consumption);

                if (oxygenProduction.CurrentValue <= Time.deltaTime)
                    oxygenProduction.SetValue(0);
                else
                    oxygenProduction.ApplyChange(-Time.deltaTime);
            }
        }

        private float GetConsumption()
        {
            float consumption = Time.deltaTime;
            if (oxygenSeconds.CurrentValue > 0)
                return consumption / consumptionReduction;

            return consumption;
        }

        [ClientRpc]
        public void IncreaseProductionTime(float increase)
        {
            if (oxygenProduction.CurrentValue + increase > oxygenProduction.maximumValue)
                oxygenProduction.SetValue(oxygenProduction.maximumValue);
            else
                oxygenProduction.ApplyChange(increase);

        }
    }
}