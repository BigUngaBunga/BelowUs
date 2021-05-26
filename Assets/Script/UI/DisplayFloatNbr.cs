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

        protected readonly float updateFrequency = 0.1f;

        protected TextMeshProUGUI text;

        private string errorString;

        private void Awake()
        {
            errorString = GetType().Name + " has unassigned variables in " + gameObject.name;
            text = (TextMeshProUGUI)textObject.GetComponent(typeof(TextMeshProUGUI));
        }

        protected virtual void OnEnable()
        {
            if (resource == null || textObject == null)
            {
                Debug.LogError(errorString);
                return;
            }

            InvokeRepeating(nameof(UpdateTextValue), 0, updateFrequency);

            if (isServer)
                resource.ApplyChange(0);
        }

        protected virtual void OnDisable() => CancelInvoke(nameof(UpdateTextValue));

        protected double GetRounded(float number) => System.Math.Round(number, decimals);

        public virtual void UpdateTextValue() => text.text = enableMaximum ? GetRounded(resource.CurrentValue) + separator + resource.MaximumValue.Value : GetRounded(resource.CurrentValue).ToString();
    }
}