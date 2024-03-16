using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class MirrorCharacterHandleWeapon : CharacterHandleWeapon
    {
        [SerializeField] private MirrorCharacterState _mirrorCharacterState;
        [SerializeField] private WeaponsDatabase weaponsDatabase;
        //[SerializeField] private ProjectileWeaponBulletSpawner _projectileWeaponBulletSpawner;
        [SerializeField] private NetworkIdentity networkIdentity;

        protected override void OnEnable()
        {
            base.OnEnable();
            _mirrorCharacterState.OnSelectedWeaponChangedAction += OnSelectedWeaponChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _mirrorCharacterState.OnSelectedWeaponChangedAction -= OnSelectedWeaponChanged;
        }

        private void OnSelectedWeaponChanged()
        {
            var weapon = weaponsDatabase.projectileWeapons.FirstOrDefault(x => x.selectedWeapon == _mirrorCharacterState.selectedWeapon);
            if (weapon == null) return;

            OriginalCreateWeapon(weapon.projectileWeapon, weapon.projectileWeapon.WeaponID);
        }

        public override void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (!networkIdentity.isLocalPlayer) return;

            var id = weaponsDatabase.projectileWeapons.FirstOrDefault(x=>x.projectileWeapon.WeaponID == weaponID);
            if (id == null) return;

            _mirrorCharacterState.CmdSetNewWeapon(id.selectedWeapon);
        }

        public void OriginalCreateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            print("change weapon on " + gameObject.name + " to " + newWeapon.WeaponID);
            // if the character already has a weapon, we make it stop shooting
            if (CurrentWeapon != null)
            {
                CurrentWeapon.TurnWeaponOff();
                if (!combo)
                {
                    ShootStop();

                    if (_weaponAim != null) { _weaponAim.RemoveReticle(); }
                    if (_character._animator != null)
                    {
                        AnimatorControllerParameter[] parameters = _character._animator.parameters;
                        foreach (AnimatorControllerParameter parameter in parameters)
                        {
                            if (parameter.name == CurrentWeapon.EquippedAnimationParameter)
                            {
                                MMAnimatorExtensions.UpdateAnimatorBool(_animator, CurrentWeapon.EquippedAnimationParameter, false);
                            }
                        }
                    }
                    Destroy(CurrentWeapon.gameObject);
                }
            }

            if (newWeapon != null)
            {
                InstantiateWeapon(newWeapon, weaponID, combo);
            }
            else
            {
                CurrentWeapon = null;
                HandleWeaponModel(null, null);
            }

            if (OnWeaponChange != null)
            {
                OnWeaponChange();
            }
        }

        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (!combo)
            {
                CurrentWeapon = (Weapon)Instantiate(newWeapon, WeaponAttachment.transform.position + newWeapon.WeaponAttachmentOffset, WeaponAttachment.transform.rotation);
            }

            CurrentWeapon.name = newWeapon.name;
            CurrentWeapon.transform.parent = WeaponAttachment.transform;
            CurrentWeapon.transform.localPosition = newWeapon.WeaponAttachmentOffset;
            CurrentWeapon.SetOwner(_character, this);
            CurrentWeapon.WeaponID = weaponID;
            CurrentWeapon.FlipWeapon();
            _weaponAim = CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();

            HandleWeaponAim();

            // we handle (optional) inverse kinematics (IK) 
            HandleWeaponIK();

            // we handle the weapon model
            HandleWeaponModel(newWeapon, weaponID, combo, CurrentWeapon);

            // we turn off the gun's emitters.
            CurrentWeapon.Initialization();
            CurrentWeapon.InitializeComboWeapons();
            CurrentWeapon.InitializeAnimatorParameters();
            InitializeAnimatorParameters();
        }
    }
}