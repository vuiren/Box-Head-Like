using Mirror;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts
{
    internal class ProjectileWeaponBulletSpawner : NetworkBehaviour
    {
        //[SerializeField] private ProjectileWeapon projectileWeapon;
        [SerializeField] private MirrorCharacterHandleWeapon handleWeapon;

        [Command]
        public void CmdCreateBullet(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation)
        {
            var projectileWeapon = handleWeapon.CurrentWeapon as ProjectileWeapon;
            if (projectileWeapon == null) return;
            projectileWeapon.SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);
        }

        [Command]
        public void CmdChangeWeapon(SelectedWeapon selectedWeapon)
        {
            print("execute");
            handleWeapon.OriginalCreateWeapon(handleWeapon.InitialWeapon, handleWeapon.InitialWeapon.WeaponID);
        }
    }
}
