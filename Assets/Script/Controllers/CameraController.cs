using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform player = null;
        private Transform submarine;
        [SerializeField] private Vector3 offsetPlayer;
        [SerializeField] private Vector3 offsetSubmarine;
        [SerializeField] private bool followPlayer;

        [Range(2, 16)]
        [SerializeField] private float smoothFactor;

        [TagSelector] [SerializeField] private string playerTag;
        [TagSelector] [SerializeField] private string submarineTag;

        private void Awake()
        {
            InvokeRepeating(nameof(FindPlayer), 0.25f, 0.25f);
            InvokeRepeating(nameof(FindSubmarine), 0.25f, 0.25f);
        }

        private void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

            if (playerObj != null)
            {
                player = playerObj.transform;
                CancelInvoke(nameof(FindPlayer));
            }
        }

        private void FindSubmarine()
        {
            GameObject submarineObj = GameObject.FindGameObjectWithTag(submarineTag);

            if (submarineObj != null)
            {
                submarine = submarineObj.transform;
                CancelInvoke(nameof(FindSubmarine));
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

        //TODO ta bort då vanliga SwitchTarget fungerar, detta är enbart för att få speltestet att fungera
        public void SwitchTarget(bool switchToPlayer) => followPlayer = switchToPlayer;
    }
}
