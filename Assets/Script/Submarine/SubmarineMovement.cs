using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class SubmarineMovement : NetworkBehaviour
    {
        [SerializeField] private StationController subController;
        [SerializeField] private GameObject stations;

        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;
        private ShipResource enginePower;
        private FlipSubmarineComponent[] submarineComponents;
        private float subSpeed;
        private float submarineRotationSpeed;
        private float angularRetardation, lateralRetardation;

        public bool IsFlipped { get; private set; }
        private bool MoveSubmarine => subController.StationPlayerController != null && NetworkClient.localPlayer == subController.StationPlayerController;
        private bool EngineIsRunning => enginePower.CurrentValue > 0;
        
        private void Start()
        {
            subController = stations.GetComponentInChildren<SubController>();
            enginePower = GameObject.Find("EnginePower").GetComponent<ShipResource>();
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            submarineComponents = GetComponentsInChildren<FlipSubmarineComponent>();
            subSpeed = 40;
            submarineRotationSpeed = 0.75f;
            angularRetardation = 0.033f;
            lateralRetardation = 0.040f;
        }

        private void FixedUpdate()
        {
            float rotation = Rotate();
            float speed = Move();

            if (isServer)
                HandleMovementAndRotation(rotation, speed);
            else
                CommandHandleMovementAndRotation(rotation, speed);
        }

        private void Update()
        {
            if (EngineIsRunning && MoveSubmarine)
                FlipSubmarine();
        }

        private float Rotate()
        {
            if (MoveSubmarine && EngineIsRunning)
            {
                if ((transform.rotation.eulerAngles.z <= 90 || transform.rotation.eulerAngles.z >= 100) && (Input.GetButton("ReverseRight") || Input.GetButton("RotateRight")))
                    return submarineRotationSpeed;
                    //transform.Rotate(0, 0, submarineRotationSpeed);

                if ((transform.rotation.eulerAngles.z <= 100 || transform.rotation.eulerAngles.z >= 270) && (Input.GetButton("ReverseLeft") || Input.GetButton("RotateLeft")))
                    return -submarineRotationSpeed;
                    //transform.Rotate(0, 0, -submarineRotationSpeed);
            }
            return 0;
        }

        private float Move()
        {
            if (MoveSubmarine)
            {
                if (EngineIsRunning && Input.GetButton("MoveForward"))
                    return subSpeed;
                //rb2D.AddForce(transform.right * subSpeed, ForceMode2D.Force);
                if (EngineIsRunning && Input.GetButton("MoveBackwards"))
                    return -subSpeed;
                    //rb2D.AddForce(-transform.right * subSpeed, ForceMode2D.Force);
            }
            return 0;
        }

        [Command]
        private void CommandHandleMovementAndRotation(float rotation, float speed) => HandleMovementAndRotation(rotation, speed);

        [Server] 
        private void HandleMovementAndRotation(float rotation, float speed)
        {
            transform.Rotate(0, 0, rotation);
            rb2D.AddForce(transform.right * speed, ForceMode2D.Force);
            MovementAndRotationRetardation();
        }

        private void FlipSubmarine()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsFlipped = !IsFlipped;
                spriteRenderer.flipX = IsFlipped;
                subSpeed *= -1;              
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.eulerAngles.z));
                foreach (FlipSubmarineComponent component in submarineComponents)
                    component.FlipObject(IsFlipped);
            }
        }

        //TODO Try calculating before Server and Command methods
        private void MovementAndRotationRetardation()
        {
            rb2D.velocity = !EngineIsRunning || (!Input.GetButton("MoveForward") && !Input.GetButton("MoveBackwards"))
                ? new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation))
                : new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation / 10), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation / 10));
            rb2D.angularVelocity = Mathf.Lerp(rb2D.angularVelocity, 0, angularRetardation);
        }
    }
}