using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class BaseStationController : NetworkBehaviour
    {
        
        [SerializeField] [SyncVar] protected NetworkIdentity stationPlayerController = null;
        public NetworkIdentity StationPlayerController => stationPlayerController;
        public bool IsOccupied => stationPlayerController != null;
    }
}




