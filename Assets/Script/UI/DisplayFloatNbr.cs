using Mirror;
using TMPro;
using UnityEngine;

public class DisplayFloatNbr : NetworkBehaviour
{
    [SerializeField] private ShipResource Variable;
    [SerializeField] private bool EnableMaximum;
    [SerializeField] private string Separator = "/";
    [SerializeField] private GameObject TextObject;
    private TextMeshProUGUI Text;

    private void OnEnable()
    {
        Variable.EventResourceChanged += HandleResourceChanged;
    }

    private void OnDisable()
    {
        Variable.EventResourceChanged -= HandleResourceChanged;
    }

    private void Awake()
    {
        Text = (TextMeshProUGUI)TextObject.GetComponent(typeof(TextMeshProUGUI));
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
