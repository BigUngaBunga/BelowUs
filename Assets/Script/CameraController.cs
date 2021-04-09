using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform submarine;
    [SerializeField] private Vector3 offsetPlayer;
    [SerializeField] private Vector3 offsetSubmarine;
    [SerializeField] private bool followPlayer;

    [Range(2, 16)]
    [SerializeField] private float smoothFactor;

    private void Start()
    {
        InvokeRepeating(nameof(FindPlayer), 0, 0.01f);
    }

    private void FindPlayer()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        GameObject playerGameObject = null;

        for (int i = 0; i < playerGameObjects.Length; i++) //Iterates through all players and checks if any of them are the local player
            if (playerGameObjects[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                playerGameObject = playerGameObjects[i];
                break;
            }

        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
            CancelInvoke(nameof(FindPlayer));
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = CalculateTargetPosition();
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
            transform.position = smoothedPosition;
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        if (followPlayer)
            return player.position + offsetPlayer;
        else
            return submarine.position + offsetSubmarine;
    }

    public void SwitchTarget()
    {
        followPlayer = !followPlayer;
        transform.position = CalculateTargetPosition();
    }
}
