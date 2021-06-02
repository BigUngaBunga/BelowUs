using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

namespace BelowUs
{
    public class UpgradeHandler : NetworkBehaviour
    {
        [SerializeReference] private TextMeshProUGUI costText;
        [SerializeReference] private TextMeshProUGUI valueText;
        [SerializeReference] private TextMeshProUGUI upgradeAmmountText;
        [SerializeReference] private ShopResource shopResource;
        [SerializeReference] private VillageResource gold;



        private void Start()
        {
            if (isServer)
                UpdateUI();
            else
                CommandUpdateUI();
        }

        public void AttemptUpgrade()
        {
            if (isServer)
                Upgrade();
            else
                CommandUpgrade();
        }
        
        [Server]
        private void Upgrade()
        {
            float price = shopResource.Price;

            if (gold.CurrentValue >= price)
            {
                gold.ApplyChange(-price);
                shopResource.ApplyUpgrade();
                UpdateUI();
                Invoke(nameof(UpdateUI), 0.11f);
            }
        }

        [Command(requiresAuthority = false)]
        private void CommandUpgrade() => Upgrade();

        [Command(requiresAuthority = false)]
        private void CommandUpdateUI() => ServerUpdateUI();

        [Server]
        private void ServerUpdateUI() => UpdateUI();

        [ClientRpc]
        private void UpdateUI()
        {
            costText.text = $"Cost: {shopResource.Price}";
            valueText.text = $"Value: {shopResource.Value}";
            upgradeAmmountText.text = $"Buy: +{shopResource.Upgrade}";
        }
    }
}

