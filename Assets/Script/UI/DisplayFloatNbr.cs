using Mirror;
using TMPro;
using UnityEngine;

public class DisplayFloatNbr : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ShipResource Resource;
    [SerializeField] private GameObject TextObject;
    [SerializeField] private FloatReference StartUpdateDelay;

    [Header("Options")]
    [SerializeField] private bool EnableMaximum;
    [SerializeField] private string Separator = "/";


    private TextMeshProUGUI Text;

    private void OnEnable()
    {
        Resource.EventResourceChanged += HandleResourceChanged;
        Invoke(nameof(UpdateBarFill), StartUpdateDelay.Value);
    }

    private void OnDisable()
    {
        Resource.EventResourceChanged -= HandleResourceChanged;
    }

    private void Awake()
    {
        Text = (TextMeshProUGUI)TextObject.GetComponent(typeof(TextMeshProUGUI));
    }

    private void UpdateBarFill()
    {
        if (EnableMaximum)
            Text.text = Resource.CurrentValue + Separator + Resource.MaximumValue.Value;
        else
            Text.text = Resource.CurrentValue.ToString();
    }

    [ClientRpc]
    public void HandleResourceChanged(float currentValue, float maxValue)
    {
        if (EnableMaximum)
            Text.text = currentValue + Separator + maxValue;
        else
            Text.text = currentValue.ToString();
    }
}
