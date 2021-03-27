using Mirror;
using UnityEngine;

public class ShipResource : NetworkBehaviour
{
    #if UNITY_EDITOR
    public string ResourceName = "";
    #endif

    [SyncVar] public float CurrentValue = 100;
    [SerializeField] private bool ResetValue;
    [SerializeField] private FloatReference StartingValue;
    public FloatReference MaximumValue;

    public delegate void ResourceChangedDelegate(float currentHealth, float maxHealth);
    public event ResourceChangedDelegate EventResourceChanged;

    #region Server
    [Server]
    private void ApplyChange(float value)
    {
        CurrentValue += value;
        EventResourceChanged?.Invoke(CurrentValue, MaximumValue.Value);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        if(ResetValue)
            CurrentValue = MaximumValue.Value;
    }


    [Command]
    public void CmdDecreaseBy5() => ApplyChange(-5);

    #endregion


    #region Client

    [Client]
    public override void OnStartClient()
    {
        
    }

    #endregion
}