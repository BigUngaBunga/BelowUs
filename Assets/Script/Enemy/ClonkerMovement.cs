using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class ClonkerMovement : EnemyBase
    {

        protected override void Update()
        {
            if (isServer)
                ServerStuff();
        }

        [Server]
        private void ServerStuff()
        {
            CheckDistanceToTargetChasing();

            switch (currentState)
            {
                case EnemyState.Idle:
                    break;
                default:
                    UpdateBasicRotation(targetGameObject.transform.position);
                    UpdateMovementChasing();
                    break;
            }
            CheckForFlip();
        }

        protected void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
    }
}