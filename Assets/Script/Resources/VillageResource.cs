using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class VillageResource : ShipResource
    {
        [SerializeReference] FloatVariable floatVariable;

        public override void ApplyChange(float value)
        {
            base.ApplyChange(value);
            floatVariable.SetValue(CurrentValue);
        }
        public override void SetValue(float value)
        {
            base.SetValue(value);
            floatVariable.SetValue(CurrentValue);
        }
    }
}

