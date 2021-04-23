using Mirror;
using UnityEngine;

public class ShipResource : NetworkBehaviour
{
    #if UNITY_EDITOR
    public string ResourceName = "";
    #endif

    [SyncVar] private float currentValue;
    public float CurrentValue { get { return currentValue; } }

    [SerializeField] private bool resetValue;
    [SerializeField] private FloatReference startingValue;
    public FloatReference maximumValue;

    public delegate void ResourceChangedDelegate(float currentHealth, float maxHealth);
    public event ResourceChangedDelegate EventResourceChanged;

    #region Server
    [Server]
    private void ApplyChange(float Value)
    {
        currentValue += Value;
        EventResourceChanged?.Invoke(currentValue, maximumValue.Value);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        if(resetValue)
            currentValue = maximumValue.Value;
    }


    [Command]
    public void CmdDecreaseBy5() => ApplyChange(-5);

    #endregion
}