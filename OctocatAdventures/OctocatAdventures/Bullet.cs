using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public class Bullet
    {
        public Entity Owner { get; set; }
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Velocity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle HitBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, Width, Height); }
        }
        public int Damage { get; set; }
        public bool Active { get; set; }

        public void Update(float elapsed)
        {
            position += Velocity * elapsed;
            foreach (Entity e in Owner.Map.Entities)
            {
                if (e == Owner) continue;

                if (HitBox.Intersects(e.HitBox))
                {
                    // We have a hit!
                    e.DoDamage(Damage);
                    Active = false;
                }
            }
            // Deactivate if outside bounds
            if (!Owner.Map.Bounds.Contains((int)position.X, (int)position.Y))
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.BlankTexture, HitBox, HitBox, Color.Red);
        }
    }
}
