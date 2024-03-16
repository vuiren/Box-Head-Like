using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorProjectileState : NetworkBehaviour
    {
        [SyncVar]
        public Vector3 SyncedDirection = Vector3.left;

        [Command]
        public void CmdSetDirection(Vector3 newDirection)
        {
            SyncedDirection = newDirection;
        }
    }
}