using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctocatAdventures
{
    class Shotgun : Weapon
    {
        public Shotgun()
        {
            ShootSpeed = 1f;
            ReloadSpeed = 1f;
            ClipSize = 2;
            Ammo = ClipSize;
            Damage = 3;
        }
    }
}
