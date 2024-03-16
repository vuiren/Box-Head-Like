using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorCharacter : Character
    {
        [SerializeField] private NetworkIdentity networkIdentity;

        private void Start()
        {
            if (!networkIdentity.isLocalPlayer) return;

            var levelManager = FindObjectOfType<LevelManager>();
            levelManager.Players.Add(this);

            MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, this);
            MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
            MMGameEvent.Trigger("CameraBound");
        }

        protected override void Update()
        {
            if (!networkIdentity.isLocalPlayer) return;
            base.Update();
        }
    }
}