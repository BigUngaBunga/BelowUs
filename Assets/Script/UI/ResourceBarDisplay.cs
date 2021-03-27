using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ShipResource Resource = null;
    [Tooltip("Image to set the fill amount on.")]
    [SerializeField] private Image resourceBarImage = null;
    [SerializeField] private GameObject testButton = null;
    [SerializeField] private FloatReference StartUpdateDelay;

    private void OnEnable()
    {
        Resource.EventResourceChanged += HandleResourceChanged;
        Invoke(nameof(UpdateBarFill), StartUpdateDelay.Value);
    }

    private void OnDisable()
    {
        Resource.EventResourceChanged -= HandleResourceChanged;
    }

    private void Start()
    {
        InvokeRepeating(nameof(EnableTestButtonIfServer), 0, 1);
    }

    private void EnableTestButtonIfServer()
    {
        if (isServer || isClientOnly)
        {
            if (isServer && testButton != null)
                testButton.SetActive(true);
            CancelInvoke(nameof(EnableTestButtonIfServer));
        }
    }

    private void UpdateBarFill()
    {
        resourceBarImage.fillAmount = Resource.CurrentValue / Resource.MaximumValue.Value;
    }

    [ClientRpc]
    private void HandleResourceChanged(float currentHealth, float maxHealth)
    {
        resourceBarImage.fillAmount = currentHealth / maxHealth;
    }
}
