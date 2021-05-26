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
        [SerializeField] FloatVariable goldMultiplier;
        private ShipResource gold;

        private void Start()
        {
            if (isServer) //TODO consider if this script should be server only or if it should be handled this way (if server only click it in networkidentity)
                ServerStartupStuff();
        }

        [Server]
        private void ServerStartupStuff()
        {
            gold = GameObject.Find("CurrentGold").GetComponent<ShipResource>();
            minimumGoldValue += (int)(-transform.position.y * goldMultiplier.Value / 100);
            maximumGoldValue = (int)(-transform.position.y * goldMultiplier.Value / 40) + minimumGoldValue;
            Random random = new Random(transform.position.y.ToString().GetHashCode() + Environment.TickCount.ToString().GetHashCode());
            value = random.Next(minimumGoldValue, maximumGoldValue);
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

