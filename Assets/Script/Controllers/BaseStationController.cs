using Mirror;
using UnityEngine;

public class BaseStationController : NetworkBehaviour
{
    [SerializeField] [SyncVar] protected NetworkIdentity stationPlayerController = null;
    public bool IsOccupied => stationPlayerController != null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
