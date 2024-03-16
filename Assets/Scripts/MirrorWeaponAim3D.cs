using Mirror;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorWeaponAim3D : WeaponAim3D
    {
        private NetworkIdentity networkIdentity;

        protected override void Start()
        {
            base.Start();
            networkIdentity = GetComponentInParent<NetworkIdentity>();
        }

        protected override void Update()
        {
            if (!networkIdentity.isLocalPlayer) return;
            base.Update();
        }
    }
}