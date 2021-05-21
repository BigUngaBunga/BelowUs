using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class UpgradeHandler : NetworkBehaviour
    {
        //TODO update with ShopItems

        public enum UpgradeItem
        {
            Hull, LifeSuport, Floodlight, PassiveLight, CannonDamage, CannonLight, SubmarineControl, GoldBonus
        }

        private enum PriceCategory
        {
            Low, Medium, High
        }

        [SerializeField] private FloatVariable gold;
        [SerializeField] private FloatVariable oxygen;
        [SerializeField] private FloatVariable hull;
        [SerializeField] private FloatVariable floodlight;
        [SerializeField] private FloatVariable passiveLight;
        [SerializeField] private FloatVariable cannonDamage;
        [SerializeField] private FloatVariable cannonLight;
        [SerializeField] private FloatVariable submarineControl;
        [SerializeField] private FloatVariable goldBonus;

        private Dictionary<UpgradeItem, float> prices;
        private Dictionary<UpgradeItem, float> upgradeValues;
        private Dictionary<UpgradeItem, FloatVariable> upgradeVariables;

        [Server]
        private void Start()
        {
            prices = new Dictionary<UpgradeItem, float>();
            upgradeValues = new Dictionary<UpgradeItem, float>();
            upgradeVariables = new Dictionary<UpgradeItem, FloatVariable>();
        }

        public void PurchaseUpgrade(UpgradeItem item)
        {
            float price = prices[item];
            float upgradeValue = upgradeValues[item];
            if (gold.Value >= price)
            {
                gold.ApplyChange(-price);
                upgradeVariables[item].ApplyChange(upgradeValue);
            }
        }

        private void IncreasePrice(UpgradeItem item)
        {

        }
    }
}

