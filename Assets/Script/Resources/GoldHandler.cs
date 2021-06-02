using System;
using UnityEngine;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class GoldHandler : NetworkBehaviour
    {
        [SerializeField] private int minimumGoldValue;
        [SerializeField] private int maximumGoldValue;
        [SerializeField] private int value;
        [SerializeField] private FloatVariable goldMultiplier;
        private ShipResource gold;
        private int minimumGoldValueDepthIncrease, maximumGoldValueDepthIncrease;

        private void Start()
        {
            minimumGoldValueDepthIncrease = 100;
            maximumGoldValueDepthIncrease = 40;

            if (isServer) //TODO consider if this script should be server only or if it should be handled this way (if server only click it in networkidentity)
                ServerStartupStuff();
        }

        [Server]
        private void ServerStartupStuff()
        {
            gold = GameObject.Find("CurrentGold").GetComponent<ShipResource>();
            minimumGoldValue += (int)(-transform.position.y  / minimumGoldValueDepthIncrease);
            maximumGoldValue = (int)(-transform.position.y / maximumGoldValueDepthIncrease) + minimumGoldValue;
            Random random = new Random(transform.position.y.ToString().GetHashCode() + Environment.TickCount.ToString().GetHashCode());
            value = (int)(random.Next(minimumGoldValue, maximumGoldValue) * goldMultiplier.Value);
            UpdatePosition(transform.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isServer) //TODO consider if this script should be server only or if it should be handled this way
                ServerCollisionHandler(collision);
        }

        [Server]
        private void ServerCollisionHandler(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(ReferenceManager.Singleton.SubmarineTag))
            {
                gold.ApplyChange(value);
                RemoveGold();
            }
        }

        [ClientRpc]
        private void RemoveGold() => Destroy(gameObject);

        [ClientRpc]
        private void UpdatePosition(Vector3 serverPosition) => transform.position = serverPosition; 
    }
}

