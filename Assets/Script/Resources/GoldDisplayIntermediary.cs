using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class GoldDisplayIntermediary : MonoBehaviour
    {
        //TODO remove this intermediary and connect CurrentGold directly to the gold display

        void Start() => InvokeRepeating(nameof(UpdatePosition), 0.1f, 0.3f);

        private void UpdatePosition() => transform.position = new Vector2(0, CurrentGold.Gold);
    }
}
