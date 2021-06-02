using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class ShopResource : NetworkBehaviour
    {
        [SerializeField] private ShopItem shopItem;

        [SyncVar][SerializeField] private float value;
        [SyncVar] [SerializeField] private float price;
        [SyncVar] [SerializeField] private float upgrade;
        public float Value => value;
        public float Price => price;
        public float Upgrade => upgrade;

        #region Server
        [Server]
        public void ApplyUpgrade()
        {
            shopItem.ApplyUpgrade();
            UpdateValues();
        }

        [Server]
        public override void OnStartServer()
        {
            base.OnStartServer();
            UpdateValues();
        }
        #endregion

        public void UpdateValues()
        {
            value = shopItem.GetValue();
            price = shopItem.price;
            upgrade = shopItem.upgradeIncrease;
        }

        #region Commands
        [Command(requiresAuthority = false)] public void CommandApplyUpgrade() => ApplyUpgrade();
        #endregion
    }
}


