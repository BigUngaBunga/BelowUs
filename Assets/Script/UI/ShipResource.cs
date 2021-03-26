using Mirror;
using RoboRyanTron.Unite2017.Variables;
using UnityEngine;
using UnityEngine.Events;

public class ShipResource : NetworkBehaviour
{
    #if UNITY_EDITOR
    public string ResourceName = "";
    #endif

    [SerializeField] private FloatVariable CurrentValue;
    [SerializeField] private FloatReference MaximumValue;

    [SerializeField] private bool ResetValue;
    [SerializeField] private FloatReference StartingValue;
    [SerializeField] private UnityEvent DamageEvent;

    public delegate void ResourceChangedDelegate(float currentHealth, float maxHealth);
    public event ResourceChangedDelegate EventResourceChanged;

    //TODO remove this hardcoding and calculate damage based on projectile, collision etc.
    public int DamageTakenPerHit = 5;

    #region Server
    [Server]
    private void SetValue(float value)
    {
        CurrentValue.SetValue(value);
        EventResourceChanged?.Invoke(CurrentValue.Value, MaximumValue.Value);
    }

    [Server]
    private void ApplyChange(float value)
    {
        CurrentValue.ApplyChange(value);
        EventResourceChanged?.Invoke(CurrentValue.Value, MaximumValue.Value);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        if(ResetValue)
            CurrentValue.SetValue(StartingValue.Value);
    }

    [Command]
    public void CmdDecreaseBy5() => ApplyChange(-5);

    #endregion


    #region Client

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();
        EventResourceChanged?.Invoke(CurrentValue.Value, MaximumValue.Value);
    }

    #endregion
}