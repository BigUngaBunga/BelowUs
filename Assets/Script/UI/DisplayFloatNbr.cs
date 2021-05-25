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

        private string errorString;

        private bool debug = true;

        private void Awake()
        {
            errorString = GetType().Name + " has unassigned variables in " + gameObject.name;
            text = (TextMeshProUGUI)textObject.GetComponent(typeof(TextMeshProUGUI));
        }

        private void OnEnable()
        {
            if (resource == null || textObject == null)
            {
                Debug.LogError(errorString);
                return;
            }
			
            if (isServer)
            {
                resource.EventResourceChanged += HandleResourceChanged;
                resource.ApplyChange(0);
            }
        }

        private void OnDisable()
        {
            if (resource == null || textObject == null)
            {
                Debug.LogError(errorString);
                return;
            }

            if (isServer)
                resource.EventResourceChanged -= HandleResourceChanged;
        }

        protected double GetRounded(float number) => System.Math.Round(number, decimals);

        [ClientRpc] public virtual void HandleResourceChanged(float curValue, float maxValue) => text.text = enableMaximum ? GetRounded(curValue) + separator + maxValue : GetRounded(curValue).ToString();
    }
}