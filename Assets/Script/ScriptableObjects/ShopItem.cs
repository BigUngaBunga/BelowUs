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

        public void SetValue(float value) => Value.SetValue(value);

        public void SetValue(FloatVariable value) => Value.SetValue(value.Value);

        public void ApplyChange(float amount) => Value.ApplyChange(amount);

        public void ApplyChange(FloatVariable amount) => Value.ApplyChange(amount.Value);

        public void ApplyUpgrade()
        {
            ApplyChange(upgradeIncrease);
            IncreasePrice();
        }

        private void IncreasePrice() => price *= priceIncrease;
    }
}
