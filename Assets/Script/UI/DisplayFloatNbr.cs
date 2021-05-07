using Mirror;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class DisplayFloatNbr : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] protected ShipResource resource;
        [SerializeField] private GameObject textObject;
        [SerializeField] private FloatReference startUpdateDelay;

        [Header("Options")]
        [SerializeField] protected bool enableMaximum;


        [SerializeField] private int decimals = 0;
        [SerializeField] protected string separator = "/";
		
        protected TextMeshProUGUI text;

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

        protected virtual void UpdateBarFill() => text.text = enableMaximum ? GetRounded(resource.CurrentValue) + separator + resource.MaximumValue.Value : GetRounded(resource.CurrentValue).ToString();

        protected double GetRounded(float number) => System.Math.Round(number, decimals);

        [ClientRpc]
        public virtual void HandleResourceChanged(float currentValue, float maxValue) => text.text = enableMaximum ? GetRounded(currentValue) + separator + maxValue : GetRounded(currentValue).ToString();
    }
}