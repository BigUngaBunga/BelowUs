using UnityEngine;

namespace BelowUs
{
    public class LightDimmer : MonoBehaviour
    {
        [SerializeReference] private Transform target;
        private new Light light;
        private float startingIntensity;
        private void Start()
        {
            light = GetComponentInParent<Light>();
            startingIntensity = light.intensity;
            InvokeRepeating("DimmLight", 0.1f, 0.1f);
        }

        private void DimmLight()
        {
            float newIntensity = startingIntensity - LogarithmOfDepth();
            light.intensity = newIntensity > 1 ? 1 : newIntensity;
        }

        private float LogarithmOfDepth()
        {
            return Mathf.Log10(-target.position.y / 10) > 0 ? Mathf.Log10(-target.position.y / 10) : 0;
        }
    }
}