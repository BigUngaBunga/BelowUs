using UnityEngine;

namespace BelowUs
{
    public class Station : MonoBehaviour
    {
        [SerializeField] private CameraController controller;
        [SerializeField] private GameObject leaveButton;
        [SerializeField] [TagSelector] private string playerTag;
        [SerializeField] [TagSelector] private string stationTag;

        private bool isTaken = false;

        public void CheckCollision(Collision2D collision)
        {
            if (!isTaken && collision.collider.CompareTag(playerTag))
            {
                isTaken = true;
                controller.SwitchTarget(stationTag);
            }
        }

        public void Leave()
        {
            isTaken = false;
            controller.SwitchTarget(playerTag);
        }
    }
}


