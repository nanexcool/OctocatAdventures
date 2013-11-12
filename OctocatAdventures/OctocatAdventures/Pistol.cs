using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctocatAdventures
{
    class Pistol : Weapon
    {
        public Pistol()
        {
            ShootSpeed = 0.25f;
            ReloadSpeed = 1f;
            ClipSize = 15;
            Ammo = ClipSize;
            Damage = 1;
        }
    }
}
