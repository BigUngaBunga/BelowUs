using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class OxygenHandler : NetworkBehaviour
    {
        [SerializeReference] private ShipResource oxygenProduction;
        [SerializeField] private float consumptionReduction;
        [SerializeField] [SyncVar] private ShipResource oxygenSeconds;
        private float repeatTime;

        private void Start()
        {
            if (isServer)
            {
                oxygenSeconds = GetComponent<ShipResource>();
                repeatTime = 0.1f;
                InvokeRepeating(nameof(DecreaseTime), 0, repeatTime);
            }
        }

        private void DecreaseTime()
        {
                
                oxygenSeconds.ApplyChange(-GetConsumption());

                if (oxygenProduction.CurrentValue <= repeatTime)
                    oxygenProduction.SetValue(0);
                else
                    oxygenProduction.ApplyChange(-repeatTime);
        }

        private float GetConsumption() => oxygenProduction.CurrentValue > 0 ? repeatTime / consumptionReduction : repeatTime;

        public void IncreaseProductionTime(float increase)
        {
            if (oxygenProduction.CurrentValue + increase > oxygenProduction.maximumValue)
                oxygenProduction.SetValue(oxygenProduction.maximumValue);
            else
                oxygenProduction.ApplyChange(increase);
        }
    }
}