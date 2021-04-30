using UnityEngine;

namespace BelowUs
{
    public class CurrentGold : MonoBehaviour
    {
        public static int Gold { get => currentGold; set { currentGold = value < 0 ? 0 : value; } }
        private static int currentGold;
    }
}

