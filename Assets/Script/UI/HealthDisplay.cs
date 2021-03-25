using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private ShipResource health = null;

    [Tooltip("Image to set the fill amount on.")]
    [SerializeField] private Image healthBarImage = null;

    [SerializeField] private GameObject testButton = null;

    private void OnEnable()
    {
        health.EventResourceChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        health.EventResourceChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        InvokeRepeating(nameof(EnableTestButtonIfServer), 0, 1);
    }

    private void EnableTestButtonIfServer()
    {
        if (isServer || isClientOnly)
        {
            if (isServer)
                testButton.SetActive(true);
            CancelInvoke(nameof(EnableTestButtonIfServer));
        }
    }

    [ClientRpc]
    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
