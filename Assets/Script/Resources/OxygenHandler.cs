using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class OxygenHandler : NetworkBehaviour
    {
        [SerializeReference] private ShipResource oxygenProduction;
        [SerializeField] private float consumptionReduction;
        [SerializeField] [SyncVar] private ShipResource oxygenSeconds;

        private void Start()
        {
            if (isServer)
                oxygenSeconds = GetComponent<ShipResource>();
        }

        private void Update()
        {
            //TODO onödigt att köra denna if satsen om och om igen på en klient kör en invokerepeating eller nått ifall det går
            if (isServer)
                DecreaseTime(GetConsumption());
        }

        private void DecreaseTime(float consumption)
        {
                oxygenSeconds.ApplyChange(-consumption);

                if (oxygenProduction.CurrentValue <= Time.deltaTime)
                    oxygenProduction.SetValue(0);
                else
                    oxygenProduction.ApplyChange(-Time.deltaTime);
        }

        private float GetConsumption()
        {
            float consumption = Time.deltaTime;
            if (oxygenSeconds.CurrentValue > 0)
                return consumption / consumptionReduction;

            return consumption;
        }

        public void IncreaseProductionTime(float increase)
        {
            if (oxygenProduction.CurrentValue + increase > oxygenProduction.maximumValue)
                oxygenProduction.SetValue(oxygenProduction.maximumValue);
            else
                oxygenProduction.ApplyChange(increase);
        }
    }
}