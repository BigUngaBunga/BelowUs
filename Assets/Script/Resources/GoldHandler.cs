using System;
using UnityEngine;
using Random = System.Random;

namespace BelowUs
{
    public class GoldHandler : MonoBehaviour
    {
        [SerializeField] int minimumGoldValue;
        [SerializeField] int maximumGoldValue;
        [SerializeField] private int value;

        private void Start()
        {
            minimumGoldValue += (int)(-transform.position.y / 100);
            maximumGoldValue = (int)(-transform.position.y / 40) + minimumGoldValue;
            Random random = new Random(transform.position.y.ToString().GetHashCode() + Environment.TickCount.ToString().GetHashCode());
            value = random.Next(minimumGoldValue, maximumGoldValue);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Submarine")
            {
                CurrentGold.Gold += value;
                Destroy(gameObject);
            }
        }
    }
}

