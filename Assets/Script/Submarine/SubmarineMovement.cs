using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class SubmarineMovement : NetworkBehaviour
    {
        [SerializeField] private StationController subController;
        [SerializeField] private GameObject stations;

        [SyncVar] private float subSpeed;
        [SyncVar] private bool isFlipped;


        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;
        private ShipResource enginePower;
        private FlipSubmarineComponent[] submarineComponents;
        
        private float submarineRotationSpeed;
        private float angularRetardation, lateralRetardation;

        public bool IsFlipped => isFlipped;
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

            if (MoveSubmarine && EngineIsRunning)
            {
                if (isServer)
                    HandleMovementAndRotation(rotation, speed);
                else
                    CommandHandleMovementAndRotation(rotation, speed);
            }

            if ((isServer && subController.StationPlayerController == null) || (isServer && MoveSubmarine))
                MovementAndRotationRetardation(speed);
            else if (MoveSubmarine)
                CommandMovementAndRotationRetardation(speed);
        }

        private void Update()
        {
            if (EngineIsRunning && MoveSubmarine)
            {
                if (isServer)
                    FlipSubmarine();
                else
                    CommandFlipSubmarine();
            }
                
        }

        private float Rotate()
        {
            if ((transform.rotation.eulerAngles.z <= 90 || transform.rotation.eulerAngles.z >= 100) && (Input.GetButton("ReverseRight") || Input.GetButton("RotateRight")))
                return submarineRotationSpeed;

            if ((transform.rotation.eulerAngles.z <= 100 || transform.rotation.eulerAngles.z >= 270) && (Input.GetButton("ReverseLeft") || Input.GetButton("RotateLeft")))
                return -submarineRotationSpeed;
            return 0;
        }

        private float Move()
        {
            if (EngineIsRunning && Input.GetButton("MoveForward"))
                return subSpeed;
            if (EngineIsRunning && Input.GetButton("MoveBackwards"))
                return -subSpeed;
            return 0;
        }

        [Server] 
        private void HandleMovementAndRotation(float rotation, float speed)
        {
            if (rotation != 0)
                transform.Rotate(0, 0, rotation);
                
            if (speed != 0)
                rb2D.AddForce(transform.right * speed, ForceMode2D.Force);
        }
        
        [Command]
        private void CommandHandleMovementAndRotation(float rotation, float speed) => HandleMovementAndRotation(rotation, speed);

        //TODO synchronize submarine flip
        [Server]
        private void FlipSubmarine()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isFlipped = !IsFlipped;
                subSpeed *= -1;
                FlipSubmarineOnAllClients();
            }
        }

        [ClientRpc]
        private void FlipSubmarineOnAllClients()
        {
            spriteRenderer.flipX = IsFlipped;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.eulerAngles.z));
            //foreach (FlipSubmarineComponent component in submarineComponents)
            //    component.FlipObject(IsFlipped);
        }

        [Command]
        private void CommandFlipSubmarine() => FlipSubmarine();

        [Server]
        private void MovementAndRotationRetardation(float speed)
        {
            rb2D.velocity = EngineIsRunning && speed != 0
                ? new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation / 10), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation / 10))
                : new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, lateralRetardation), Mathf.Lerp(rb2D.velocity.y, 0, lateralRetardation));
            rb2D.angularVelocity = Mathf.Lerp(rb2D.angularVelocity, 0, angularRetardation);
        }
        [Command]
        private void CommandMovementAndRotationRetardation(float speed) => MovementAndRotationRetardation(speed);
    }
}