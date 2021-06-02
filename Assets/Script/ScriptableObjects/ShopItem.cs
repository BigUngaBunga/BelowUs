using UnityEngine;

namespace BelowUs
{
    [CreateAssetMenu]
    public class ShopItem : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField] private string DeveloperDescription = "";
#endif

        [SerializeField] public FloatVariable Value;
        [SerializeField] public float price, upgradeIncrease, priceIncrease;

        public float GetValue() => Value.Value;

        public void SetValue(float value) => Value.SetValue(value);

        public void ApplyChange(float amount) => Value.ApplyChange(amount);

        public void ApplyUpgrade()
        {
            ApplyChange(upgradeIncrease);
            IncreasePrice();
        }

        private void IncreasePrice() => price *= priceIncrease;
    }
}
