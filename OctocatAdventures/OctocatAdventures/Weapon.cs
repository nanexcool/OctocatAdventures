using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace OctocatAdventures
{
    public enum BulletType
    {
        Regular,
        Piercing
    }

    public enum WeaponState
    {
        Empty,
        Reloading,
        Ready
    }

    public abstract class Weapon
    {
        public static SoundEffect ReloadSound { get; set; }
        //public static SoundEffect EmptyShootSound { get; set; }

        public static SoundEffectInstance EmptyShootSound { get; set; }

        public int Ammo { get; set; }
        public int ClipSize { get; set; }

        public int Damage { get; set; }

        public float ShootSpeed { get; set; }
        public float ReloadSpeed { get; set; }

        public WeaponState State { get; set; }

        public float reloadTimer = 0;

        protected Weapon()
        {
            State = WeaponState.Ready;
        }

        public virtual void Update(float elapsed)
        {
            if (State == WeaponState.Reloading)
            {
                reloadTimer += elapsed;
                if (reloadTimer >= ReloadSpeed)
                {
                    Ammo = ClipSize;
                    State = WeaponState.Ready;
                    reloadTimer = 0;
                }
            }
        }

        public virtual bool Shoot()
        {
            switch (State)
            {
                case WeaponState.Empty:
                    break;
                case WeaponState.Reloading:
                    break;
                case WeaponState.Ready:
                    Ammo--;
                    if (Ammo <= 0)
                    {
                        State = WeaponState.Empty;
                    }
                    return true;
                default:
                    break;
            }
            return false;
        }

        public virtual void Reload()
        {
            if (State != WeaponState.Reloading && Ammo < ClipSize)
            {
                State = WeaponState.Reloading;
                ReloadSound.Play();
            }
        }
    }
}
