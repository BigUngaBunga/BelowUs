using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class ShipResource : NetworkBehaviour
    {
        //#if UNITY_EDITOR
        [SerializeField] private string resourceName = "";
        //#endif
        public string ResourceName => resourceName;

        [SyncVar] private float currentValue;
        public float CurrentValue => currentValue;

        [SerializeField] private bool resetValue;
        [SerializeField] private FloatReference maximumValue;
        public FloatReference MaximumValue => maximumValue;

        public delegate void ResourceChangedDelegate(float currentHealth, float maxHealth);
        public event ResourceChangedDelegate EventResourceChanged;

        public delegate void ResourceEmptyDelegate();
        public event ResourceEmptyDelegate EventResourceEmpty;

        [SerializeField] private bool debug;
        [SerializeField] private bool nullify;

        #region Server
        [Server]
        public void ApplyChange(float value)
        {
            if (debug)
                Debug.Log(gameObject.name + " " + nameof(currentValue) + " is " + currentValue + " before " + value + " change");

            currentValue = Mathf.Clamp(currentValue + value, 0, maximumValue.Value);
            if (nullify)
                currentValue = 0;

            if (debug)
                Debug.Log(gameObject.name + " " + nameof(currentValue) + " is " + currentValue + " after " + value + " change");

            EventResourceChanged?.Invoke(currentValue, maximumValue.Value);

            if (currentValue == 0)
                EventResourceEmpty?.Invoke();
        }

        [Server]
        public void SetValue(float value)
        {
            currentValue = value;
            EventResourceChanged?.Invoke(currentValue, maximumValue.Value);

            if (currentValue == 0)
                EventResourceEmpty?.Invoke();
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
