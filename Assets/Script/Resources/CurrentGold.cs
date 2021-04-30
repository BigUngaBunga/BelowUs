using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public static class CurrentGold
    {
        public static int Gold { get => currentGold; set { currentGold = value < 0 ? 0 : value; } }
        private static int currentGold;
    }
}

