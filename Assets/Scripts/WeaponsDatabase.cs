using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class WeaponIdTuple
    {
        public SelectedWeapon selectedWeapon;
        public ProjectileWeapon projectileWeapon;
    }

    [CreateAssetMenu(fileName = "Weapons Database", menuName = "ScriptableObjects/Weapons Database", order = 1)]
    public class WeaponsDatabase : ScriptableObject
    {
        public WeaponIdTuple[] projectileWeapons;
    }
}