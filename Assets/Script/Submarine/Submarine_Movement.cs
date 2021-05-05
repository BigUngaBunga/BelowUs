using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class Submarine_Movement : MonoBehaviour
    {
        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;
        private float subSpeed;
        private float submarineRotationSpeed;
        float angularRetardation, lateralRetardation;
        public bool IsFlipped { get; private set; }
        private bool MoveSubmarine => subController.StationPlayerController != null && ClientScene.localPlayer.gameObject == subController.StationPlayerController;

        [SerializeField] private StationController subController;
        

        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            subSpeed = 50;
            submarineRotationSpeed = 0.5f;
            angularRetardation = 0.033f;
            lateralRetardation = 0.1f;
        }

        private void FixedUpdate()
        {
            if (MoveSubmarine)
            {
                HandleRotation();
                HandleLateralMovement();
                StopCollisionAngularMomentum();
            }
        }

        private void Update()
        {
            if (MoveSubmarine)
                FlipSubmarine();
        }

        private void HandleRotation()
        {
            if ((transform.rotation.eulerAngles.z <= 90 || transform.rotation.eulerAngles.z >= 100) && (Input.GetButton("ReverseRight") || Input.GetButton("RotateRight")))
                transform.Rotate(0, 0, submarineRotationSpeed);

            if ((transform.rotation.eulerAngles.z <= 100 || transform.rotation.eulerAngles.z >= 270) && (Input.GetButton("ReverseLeft") || Input.GetButton("RotateLeft")))
                transform.Rotate(0, 0, -submarineRotationSpeed);
        }

        private void HandleLateralMovement()
        {
            if (Input.GetButton("MoveForward"))
                rb2D.AddForce(transform.right * subSpeed, ForceMode2D.Force);
            if (Input.GetButton("MoveBackwards"))
                rb2D.AddForce(-transform.right * subSpeed, ForceMode2D.Force);

            if (!Input.GetButton("MoveForward") && !Input.GetButton("MoveBackwards"))
                rb2D.velocity = new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation));
            else
                rb2D.velocity = new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation / 10), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation / 10));

        }

        private void FlipSubmarine()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsFlipped = !IsFlipped;
                spriteRenderer.flipX = IsFlipped;
                subSpeed *= -1;              
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.eulerAngles.z));
            }
        }

        private void StopCollisionAngularMomentum() => rb2D.angularVelocity = Mathf.Lerp(rb2D.angularVelocity, 0, angularRetardation);
    }
}