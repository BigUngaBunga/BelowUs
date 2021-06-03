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
        private Camera cameraa;

        [Range(2, 16)]
        [SerializeField] private float smoothFactor;

        [SerializeField] private float submarineCameraSize = 14;
        [SerializeField] protected float playerCameraSize;

        private void Start()
        {
            InvokeRepeating(nameof(FindPlayer), 0.25f, 0.25f);
            InvokeRepeating(nameof(FindSubmarine), 0.25f, 0.25f);
            cameraa = GetComponentInParent<Camera>();
            playerCameraSize = cameraa.orthographicSize;
        }

        private void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag);

            if (playerObj != null)
            {
                player = playerObj.transform;
                CancelInvoke(nameof(FindPlayer));
            }
        }

        private void FindSubmarine()
        {
            GameObject submarineObj = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.SubmarineTag);

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

        public void SwitchTarget()
        {
            followPlayer = !followPlayer;
            cameraa.orthographicSize = followPlayer ? playerCameraSize : submarineCameraSize;
        }
    }
}
