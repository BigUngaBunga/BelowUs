using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class OxygenHandler : MonoBehaviour
    {
        [SerializeReference] private ShipResource oxygenProduction;
        [SerializeField] private float consumptionReduction;
        private ShipResource oxygenSeconds;

        private void Start() => oxygenSeconds = GetComponentInParent<ShipResource>();


        private void Update() => DecreaseTime(GetConsumption());

        private void DecreaseTime(float consumption)
        {
            oxygenSeconds.CmdChangeByValue(-consumption);
            oxygenProduction.CmdChangeByValue(Time.deltaTime);
            Debug.Log("Made change to oxygen " + oxygenSeconds.CurrentValue);
        }

        private float GetConsumption()
        {
            float consumption = Time.deltaTime;
            if (oxygenSeconds.CurrentValue > 0)
                consumption /= consumptionReduction;

            return consumption;
        }
    }
}