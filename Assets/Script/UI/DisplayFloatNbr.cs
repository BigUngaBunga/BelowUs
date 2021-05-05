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
        [SerializeField] private bool roundOffNumber;
        [SerializeField] private string separator = "/";



        private TextMeshProUGUI text;

        private void OnEnable()
        {
            resource.EventResourceChanged += HandleResourceChanged;
            Invoke(nameof(UpdateBarFill), startUpdateDelay.Value);
        }

        private void OnDisable() => resource.EventResourceChanged -= HandleResourceChanged;

        private void Awake() => text = (TextMeshProUGUI)textObject.GetComponent(typeof(TextMeshProUGUI));

        private void UpdateBarFill() => text.text = enableMaximum ? DisplayOneDecimal(resource.CurrentValue) + separator + resource.maximumValue.Value : DisplayOneDecimal(resource.CurrentValue);

        private string DisplayOneDecimal(float number)
        {
            if (roundOffNumber)
            {
                string value = number.ToString();
                if (value.Contains(","))
                    return value.Substring(0, value.IndexOf(',') + 2);
            }

            return number.ToString();
        }

        [ClientRpc]
        public void HandleResourceChanged(float currentValue, float maxValue) => text.text = enableMaximum ? DisplayOneDecimal(currentValue) + separator + maxValue : DisplayOneDecimal(currentValue);
    }
}