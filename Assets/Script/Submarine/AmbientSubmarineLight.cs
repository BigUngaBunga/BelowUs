using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class AmbientSubmarineLight: MonoBehaviour
    {
        [SerializeField] private FloatVariable range;
        void Start() => GetComponent<Light>().range = range.Value;
    }
}

