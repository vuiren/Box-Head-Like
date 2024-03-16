using Mirror;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorProjectile : Projectile
    {
        [SerializeField] private MirrorProjectileState _mirrorProjectileState;

        private Vector3 SyncedDirection => _mirrorProjectileState.SyncedDirection;

        public override void Movement()
        {
            _movement = (Speed / 10) * Time.deltaTime * SyncedDirection;
            //transform.Translate(_movement,Space.World);
            if (_rigidBody != null)
            {
                _rigidBody.MovePosition(this.transform.position + _movement);
            }
            if (_rigidBody2D != null)
            {
                _rigidBody2D.MovePosition(this.transform.position + _movement);
            }
            // We apply the acceleration to increase the speed
            Speed += Acceleration * Time.deltaTime;
        }

        public override void SetDirection(Vector3 newDirection, Quaternion newRotation, bool spawnerIsFacingRight = true)
        {
            _spawnerIsFacingRight = spawnerIsFacingRight;

            if (DirectionCanBeChangedBySpawner)
            {
                _mirrorProjectileState.SyncedDirection = newDirection;
            }
            if (ProjectileIsFacingRight != spawnerIsFacingRight)
            {
                Flip();
            }
            if (FaceDirection)
            {
                transform.rotation = newRotation;
            }

            if (_damageOnTouch != null)
            {
                _damageOnTouch.SetKnockbackScriptDirection(newDirection);
            }

            if (FaceMovement)
            {
                switch (MovementVector)
                {
                    case MovementVectors.Forward:
                        transform.forward = newDirection;
                        break;
                    case MovementVectors.Right:
                        transform.right = newDirection;
                        break;
                    case MovementVectors.Up:
                        transform.up = newDirection;
                        break;
                }
            }
        }
    }
}