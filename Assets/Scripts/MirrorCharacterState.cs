using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public enum SelectedWeapon
    {
        None,
        Rifle
    }

    public class MirrorCharacterState : NetworkBehaviour
    {
        public Action OnSelectedWeaponChangedAction { get; set; }
        [SyncVar(hook = "OnSelectedWeaponChanged")]
        public SelectedWeapon selectedWeapon;

        public void OnSelectedWeaponChanged(SelectedWeapon oldValue, SelectedWeapon newValue)
        {
            print("selected weapon changed: " + oldValue + " -> " + newValue);
            selectedWeapon = newValue;
            OnSelectedWeaponChangedAction?.Invoke();
        }

        [Command]
        public void CmdSetNewWeapon(SelectedWeapon selectedWeapon)
        {
            this.selectedWeapon = selectedWeapon;
        }
    }
}