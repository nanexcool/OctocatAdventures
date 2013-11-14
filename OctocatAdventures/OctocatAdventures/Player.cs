using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public class Player : Entity
    {
        float shootTimer = 0;
        bool canShoot = true;

        public List<Weapon> Weapons { get; set; }
        public Weapon EquippedWeapon { get; set; }
        int weaponIndex = 0;

        public List<Bullet> Bullets { get; set; }

        public Player(Texture2D texture)
            : base(texture)
        {
            Weapons = new List<Weapon>(5);
            Bullets = new List<Bullet>(10);

            Weapons.Add(new Pistol());
            Weapons.Add(new Shotgun());

            EquippedWeapon = Weapons[weaponIndex];
        }

        public void ChangeWeapon()
        {
            if (EquippedWeapon.State != WeaponState.Reloading)
            {
                if (++weaponIndex >= Weapons.Count)
                {
                    weaponIndex = 0;
                }
                EquippedWeapon = Weapons[weaponIndex];
            }
        }

        public void Shoot(Vector2 direction)
        {
            if (canShoot && EquippedWeapon.Shoot())
            {
                Vector2 p;
                if (direction.X == -1)
                {
                    p = new Vector2(position.X, position.Y + Width / 2);
                }
                else if (direction.X == 1)
                {
                    p = new Vector2(position.X + Width, position.Y + Width / 2);
                }
                else
                {
                    p = new Vector2(position.X + Width / 2, position.Y);
                }
                Bullet b = new Bullet()
                {
                    Owner = this,
                    Active = true,
                    Position = p,
                    Velocity = direction * 500,
                    Width = 16,
                    Height = 16,
                    Damage = EquippedWeapon.Damage
                };
                
                Bullets.Add(b);
                
                canShoot = false;
                shootTimer = 0;
            }
        }

        public void Reload()
        {
            EquippedWeapon.Reload();
        }

        public override void Update(float elapsed)
        {
            EquippedWeapon.Update(elapsed);

            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(elapsed);

                if (!Bullets[i].Active)
                {
                    Bullets.RemoveAt(i);
                    i--;
                }
            }

            if (!canShoot)
            {
                shootTimer += elapsed;
                if (shootTimer >= EquippedWeapon.ShootSpeed)
                {
                    canShoot = true;
                }
            }

            foreach (Entity e in Map.Entities)
            {
                if (e == this) continue;

                if (Collides(e))
                {
                    e.Active = false;
                }
            }
            
            base.Update(elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //string text = string.Format("{0}, {1}, {2}", EquippedWeapon.Ammo, EquippedWeapon.GetType().Name, EquippedWeapon.State);
            string text = velocity.ToString();
            spriteBatch.DrawString(Util.Font, text, new Vector2(X, Y - 24), Color.Red);
            base.Draw(spriteBatch);

            foreach (Bullet b in Bullets)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
