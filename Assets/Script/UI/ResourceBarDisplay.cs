using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class ResourceBarDisplay : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipResource resource = null;
        [Tooltip("Image to set the fill amount on.")]
        [SerializeField] private Image resourceBarImage = null;
        [SerializeField] private GameObject testButton = null;

        private readonly float updateFrequency = 0.1f;

        private void OnEnable()
        {
            if (resource == null || resourceBarImage == null)
                Debug.LogError(GetType().Name + " has unassigned variables in " + gameObject.name);

            InvokeRepeating(nameof(UpdateBarFill), 0, updateFrequency);
        }

        private void OnDisable() => CancelInvoke(nameof(UpdateBarFill));

        private void Start()
        {
            if(isServer)
                InvokeRepeating(nameof(EnableTestButtonIfServer), 0, 1);
        }

        private void EnableTestButtonIfServer()
        {
            if (testButton != null)
                testButton.SetActive(true);
            CancelInvoke(nameof(EnableTestButtonIfServer));
        }

        private void UpdateBarFill() => resourceBarImage.fillAmount = resource.CurrentValue / resource.MaximumValue.Value;
    }
}
