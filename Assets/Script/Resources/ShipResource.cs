using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class ShipResource : NetworkBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private string resourceName = "";
        #endif

        [SyncVar] private float currentValue;
        public float CurrentValue => currentValue;

        [SerializeField] private bool resetValue;
        [SerializeField] private FloatReference maximumValue;
        public FloatReference MaximumValue => maximumValue;

        public delegate void ResourceChangedDelegate(float currentHealth, float maxHealth);
        public event ResourceChangedDelegate EventResourceChanged;

        #region Server
        [Server]
        public void ApplyChange(float value)
        {
            currentValue = Mathf.Clamp(currentValue + value, 0, maximumValue.Value);
            EventResourceChanged?.Invoke(currentValue, maximumValue.Value);
        }

        [Server]
        public void SetValue(float value)
        {
            currentValue = value;
            EventResourceChanged?.Invoke(currentValue, maximumValue.Value);
        }

        [Server]
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (resetValue)
                currentValue = maximumValue.Value;
        }
        #endregion

        #region Commands
        [Command]
        public void CmdDecreaseBy5() => ApplyChange(-5);

        [Command]
        public void CmdChangeByValue(float value) => ApplyChange(value);
        #endregion
    }
}
