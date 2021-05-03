using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class CameraController : NetworkBehaviour
    {
        private Transform player;
        private Transform submarine;
        [SerializeField] private Vector3 offsetPlayer;
        [SerializeField] private Vector3 offsetSubmarine;
        [SerializeField] private bool followPlayer;

        [Range(2, 16)]
        [SerializeField] private float smoothFactor;

        [TagSelector] [SerializeField] private string playerTag;
        [TagSelector] [SerializeField] private string submarineTag;

        private void Start()
        {
            InvokeRepeating(nameof(FindPlayer), 0, 0.01f);
            submarine = GameObject.FindGameObjectWithTag(submarineTag).transform;
        }

        private void FindPlayer()
        {
            GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag(playerTag);
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
            if (player != null || !followPlayer)
            {
                Vector3 targetPosition = CalculateTargetPosition();
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
                transform.position = smoothedPosition;
            }
        }

        private Vector3 CalculateTargetPosition() => followPlayer ? player.position + offsetPlayer : submarine.position + offsetSubmarine;

        public void SwitchTarget(string targetTag)
        {
            if (targetTag == playerTag)
                followPlayer = true;
            else if (targetTag == submarineTag)
                followPlayer = false;
        }
    }

}