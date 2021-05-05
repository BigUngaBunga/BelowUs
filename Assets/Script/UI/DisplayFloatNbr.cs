using Mirror;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class DisplayFloatNbr : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipResource resource;
        [SerializeField] private GameObject textObject;
        [SerializeField] private FloatReference startUpdateDelay;

        [Header("Options")]
        [SerializeField] private bool enableMaximum;

        [SerializeField] private int decimals = 0;
        [SerializeField] private string separator = "/";
		
        private TextMeshProUGUI text;

        private void OnEnable()
        {
            if (resource == null)
            {
                Debug.Log(nameof(resource) + " is unassigned in " + gameObject);
                return;
            }
			
            resource.EventResourceChanged += HandleResourceChanged;
            Invoke(nameof(UpdateBarFill), startUpdateDelay.Value);
        }

        private void OnDisable()
        {
            if (resource == null)
            {
                Debug.Log(nameof(resource) + " is unassigned in " + gameObject);
                return;
            }

            resource.EventResourceChanged -= HandleResourceChanged;
        }

        private void Awake() => text = (TextMeshProUGUI)textObject.GetComponent(typeof(TextMeshProUGUI));

        private void UpdateBarFill() => text.text = enableMaximum ? GetRounded(resource.CurrentValue) + separator + resource.maximumValue.Value : GetRounded(resource.CurrentValue).ToString();

        private double GetRounded(float number) => System.Math.Round(number, decimals);

        [ClientRpc]
        public void HandleResourceChanged(float currentValue, float maxValue) => text.text = enableMaximum ? GetRounded(currentValue) + separator + maxValue : GetRounded(currentValue).ToString();
    }
}