using UnityEngine;

namespace BelowUs
{
    public class Station : MonoBehaviour
    {
        [SerializeField] private CameraController controller;
        [SerializeField] private GameObject leaveButton;
        [SerializeField] [TagSelector] private string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example player or station.")]
        [SerializeField] [TagSelector] private string switchTag;
        public string PlayerTag { get { return playerTag; } }
        public string SwitchTag { get { return switchTag; } }

        private bool isTaken = false;

        public void CheckCollision(Collision2D collision)
        {
            if (!isTaken && collision.collider.CompareTag(playerTag))
            {
                isTaken = true;
                controller.SwitchTarget(switchTag);
            }
        }

        public void Leave()
        {
            isTaken = false;
            controller.SwitchTarget(playerTag);
        }
    }
}


