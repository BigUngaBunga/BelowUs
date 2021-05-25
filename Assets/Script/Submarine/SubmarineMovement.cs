using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace BelowUs
{
    public class SubmarineMovement : NetworkBehaviour
    {
        [SerializeField] private StationController subController;
        [SerializeField] private GameObject stations;
        [SerializeField] private float rotationSyncTolerance;
        [SerializeField] private float positionSyncTolerance;
        [Range(10, 30)]
        [SerializeField] private int syncStepAmount;
        [SyncVar] private float submarineRotation;
        [SyncVar] private Vector2 submarinePosition;
        [SerializeField] private FloatVariable movementRetardation;
        private float subSpeed;
        private Rigidbody2D rb2D;
        private SpriteRenderer spriteRenderer;
        private ShipResource enginePower;
        private FlipSubmarineComponent[] submarineComponents;
        private bool isSynchronizingRotation, isSynchronizingPosition;
        
        private float submarineRotationSpeed;
        private float MovementRetardation => movementRetardation.Value;
        public bool IsFlipped => spriteRenderer.flipX;
        private bool MoveSubmarine => subController.IsOccupied && NetworkClient.localPlayer == subController.StationPlayerController;
        private bool EngineIsRunning => enginePower.CurrentValue > 0;
        
        private void Start()
        {
            subController = stations.GetComponentInChildren<SubController>();
            enginePower = GameObject.Find("EnginePower").GetComponent<ShipResource>();
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            submarineComponents = GetComponentsInChildren<FlipSubmarineComponent>();
            subSpeed = 40;
            submarineRotationSpeed = 2f;
            submarineRotation = rb2D.rotation;
            submarinePosition = rb2D.position;

            if (isServer)
                InvokeRepeating(nameof(CheckIfNeedSynchronization), 1, syncInterval);
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

            if ((isServer && !subController.IsOccupied) || (isServer && MoveSubmarine))
                RetardMovement(speed);
            else if (MoveSubmarine)
                CommandMovementRetardation(speed);
        }

        private void Update() => HandleSubmarineFlip();

        private float Rotate()
        {
            bool canTurnLeft = rb2D.rotation % 360 < 90;
            bool canTurnRight = rb2D.rotation % 360 > -90;

            if (Input.GetButton("ReverseLeft") || Input.GetButton("RotateLeft"))
                return canTurnLeft ? submarineRotationSpeed : -submarineRotationSpeed;

            else if (Input.GetButton("ReverseRight") || Input.GetButton("RotateRight"))
                return canTurnRight ? -submarineRotationSpeed : submarineRotationSpeed;
            else
            {
                if (!canTurnLeft && canTurnRight)
                    return -submarineRotationSpeed;
                else if (!canTurnRight && canTurnLeft)
                    return submarineRotationSpeed;
            }

            return 0;
        }

        private float Move()
        {
            if (EngineIsRunning && Input.GetButton("MoveForward"))
                return subSpeed;
            return EngineIsRunning && Input.GetButton("MoveBackwards") ? -subSpeed : 0;
        }

        [Server]
        private void HandleMovementAndRotation(float rotation, float speed)
        {
            UpdateAngularVelocity(rotation);
            rb2D.AddForce(transform.right * speed, ForceMode2D.Force);
            submarineRotation = rb2D.rotation;
            submarinePosition = rb2D.position;
        }

        [Command] private void CommandHandleMovementAndRotation(float rotation, float speed) => HandleMovementAndRotation(rotation, speed);

        [ClientRpc]
        private void UpdateAngularVelocity(float rotation)
        {
            if (rb2D != null)
                rb2D.angularVelocity += rotation;
        }

        [ClientRpc]
        private void CheckIfNeedSynchronization()
        {
            if (!isServer && rb2D != null)
            {
                float rotationDifference = submarineRotation - rb2D.rotation;
                float positionDistance = submarinePosition.magnitude - rb2D.position.magnitude;
                if (!isSynchronizingRotation && (rotationDifference > rotationSyncTolerance || rotationDifference < rotationSyncTolerance))
                    StartCoroutine(SynchronizeRotation());
                if (!isSynchronizingPosition && (positionDistance > positionSyncTolerance || positionDistance < positionSyncTolerance))
                    StartCoroutine(SynchronizePosition());
            }
        }

        private IEnumerator SynchronizeRotation()
        {
            isSynchronizingRotation = true;
            float synchronizationRate = 1 / (float)syncStepAmount;
            for (int i = 0; i < syncStepAmount; i++)
            {
                rb2D.rotation = Mathf.Lerp(rb2D.rotation, submarineRotation, synchronizationRate);
                yield return null;
            }
            isSynchronizingRotation = false;
        }

        private IEnumerator SynchronizePosition()
        {
            isSynchronizingPosition = true;
            float synchronizationRate = 1 / (float)syncStepAmount;
            for (int i = 0; i < syncStepAmount; i++)
            {
                rb2D.position = new Vector2(x: Mathf.Lerp(rb2D.position.x, submarinePosition.x, synchronizationRate), 
                                            y: Mathf.Lerp(rb2D.position.y, submarinePosition.y, synchronizationRate));
                yield return null;
            }
            isSynchronizingPosition = false;
        }

        private void HandleSubmarineFlip()
        {
            if (EngineIsRunning && MoveSubmarine && Input.GetKeyDown(KeyCode.Space))
            {
                if (!isServer)
                    CommandFlipSubmarine();
                else
                    FlipSubmarine();
            }
        }

        [Command] private void CommandFlipSubmarine() => FlipSubmarine();

        [Server] private void FlipSubmarine() => FlipSubmarineOnAllClients();

        [ClientRpc]
        private void FlipSubmarineOnAllClients()
        {
            subSpeed *= -1;
            spriteRenderer.flipX = !IsFlipped;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.eulerAngles.z));
            foreach (FlipSubmarineComponent component in submarineComponents)
                component.FlipObject(IsFlipped);
        }

        [Server]
        private void RetardMovement(float speed)
        {
            rb2D.velocity = EngineIsRunning && speed != 0
                ? new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, MovementRetardation / 10), Mathf.Lerp(rb2D.velocity.y, 0, MovementRetardation / 10))
                : new Vector2(Mathf.Lerp(rb2D.velocity.x, 0, MovementRetardation), Mathf.Lerp(rb2D.velocity.y, 0, MovementRetardation));
        }

        [Command] private void CommandMovementRetardation(float speed) => RetardMovement(speed);
    }
}