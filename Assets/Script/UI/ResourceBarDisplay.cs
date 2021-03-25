using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarDisplay : NetworkBehaviour
{
    [SerializeField] private ShipResource resource = null;

    [Tooltip("Image to set the fill amount on.")]
    [SerializeField] private Image resourceBarImage = null;

    [SerializeField] private GameObject testButton = null;

    private void OnEnable()
    {
        resource.EventResourceChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        resource.EventResourceChanged -= HandleHealthChanged;
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

    [ClientRpc]
    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        resourceBarImage.fillAmount = currentHealth / maxHealth;
    }
}
