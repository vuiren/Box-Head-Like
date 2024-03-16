using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorProjectileWeapon : ProjectileWeapon
    {
        [SerializeField] private NetworkIdentity networkIdentity;
        [SerializeField] private ProjectileWeaponBulletSpawner mirrorProjectileSpawner;
        [SerializeField] private Projectile projectilePrefab;

        protected override void Start()
        {
            base.Start();
            networkIdentity = GetComponentInParent<NetworkIdentity>();
            mirrorProjectileSpawner = GetComponentInParent<ProjectileWeaponBulletSpawner>();
        }

        public override void WeaponUse()
        {
            ApplyRecoil();
            TriggerWeaponUsedFeedback();
            DetermineSpawnPosition();

            for (int i = 0; i < ProjectilesPerShot; i++)
            {
                SendSpawnBullet(SpawnPosition, i, ProjectilesPerShot, true);
                PlaySpawnFeedbacks();
            }
        }

        public void SendSpawnBullet(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation)
        {
            if (!networkIdentity.isLocalPlayer) return;

            mirrorProjectileSpawner.CmdCreateBullet(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);
        }

        public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
        {
            /// we get the next object in the pool and make sure it's not null
            GameObject nextGameObject = (GameObject)Instantiate(projectilePrefab.gameObject);
            print("Is Called On " + gameObject.name + "Id " + networkIdentity.gameObject.name);

            // mandatory checks
            if (nextGameObject == null) { return null; }
            if (nextGameObject.GetComponent<MMPoolableObject>() == null)
            {
                throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
            }
            // we position the object
            nextGameObject.transform.position = spawnPosition;
            if (_projectileSpawnTransform != null)
            {
                nextGameObject.transform.position = _projectileSpawnTransform.position;
            }
            // we set its direction

            Projectile projectile = nextGameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetWeapon(this);
                if (Owner != null)
                {
                    projectile.SetOwner(Owner.gameObject);
                }
            }
            // we activate the object
            nextGameObject.gameObject.SetActive(true);

            if (projectile != null)
            {
                if (RandomSpread)
                {
                    _randomSpreadDirection.x = UnityEngine.Random.Range(-Spread.x, Spread.x);
                    _randomSpreadDirection.y = UnityEngine.Random.Range(-Spread.y, Spread.y);
                    _randomSpreadDirection.z = UnityEngine.Random.Range(-Spread.z, Spread.z);
                }
                else
                {
                    if (totalProjectiles > 1)
                    {
                        _randomSpreadDirection.x = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.x, Spread.x);
                        _randomSpreadDirection.y = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.y, Spread.y);
                        _randomSpreadDirection.z = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.z, Spread.z);
                    }
                    else
                    {
                        _randomSpreadDirection = Vector3.zero;
                    }
                }

                Quaternion spread = Quaternion.Euler(_randomSpreadDirection);

                if (Owner == null)
                {
                    projectile.SetDirection(spread * transform.rotation * DefaultProjectileDirection, transform.rotation, true);
                }
                else
                {
                    if (Owner.CharacterDimension == Character.CharacterDimensions.Type3D) // if we're in 3D
                    {
                        projectile.SetDirection(spread * transform.forward, transform.rotation, true);
                    }
                    else // if we're in 2D
                    {
                        Vector3 newDirection = (spread * transform.right) * (Flipped ? -1 : 1);
                        if (Owner.Orientation2D != null)
                        {
                            projectile.SetDirection(newDirection, spread * transform.rotation, Owner.Orientation2D.IsFacingRight);
                        }
                        else
                        {
                            projectile.SetDirection(newDirection, spread * transform.rotation, true);
                        }
                    }
                }

                if (RotateWeaponOnSpread)
                {
                    this.transform.rotation = this.transform.rotation * spread;
                }
            }

            if (triggerObjectActivation)
            {
                if (nextGameObject.GetComponent<MMPoolableObject>() != null)
                {
                    nextGameObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
                }
            }

            NetworkServer.Spawn(nextGameObject, networkIdentity.gameObject);
            return (nextGameObject);
        }
    }
}