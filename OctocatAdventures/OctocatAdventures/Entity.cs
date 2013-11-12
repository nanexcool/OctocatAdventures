using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public class Entity
    {
        public bool Active { get; set; }
        protected Vector2 position;

        public Texture2D Texture { get; set; }
        public Vector2 Position 
        {
            get { return position; }
            set { position = value; }
        }
        public Color Color { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }

        public Rectangle? Source { get; set; }

        public Rectangle CollisionBox 
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }

        protected Rectangle hitBox;
        public Rectangle HitBox
        {
            get { return new Rectangle(X + hitBox.X, Y + hitBox.Y, hitBox.Width, hitBox.Height); }
            set { hitBox = value; }
        }        

        public Map Map { get; set; }

        // Health
        public int Health { get; set; }


        // Jumping stuff
        protected bool isJumping = false;
        protected bool onGround = false;

        // Physics
        protected Vector2 speed = new Vector2(300);

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        
        protected Vector2 velocity;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        protected Vector2 acceleration;

        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }
        

        protected Vector2 gravity = new Vector2(0, 1000);

        public Vector2 Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        public Entity(int w, int h) : 
            this(Util.BlankTexture)
        {
            Width = w;
            Height = h;
            Source = new Rectangle(X, Y, Width, Height);
        }

        public Entity(Texture2D texture)
        {
            Texture = texture;
            Color = Color.White;
            Width = Texture.Width;
            Height = Texture.Height;

            Health = 3;
            Active = true;

            hitBox = new Rectangle(0, 0, Width, Height);
        }

        public void DoDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Active = false;
        }

        public void Move(Vector2 direction)
        {
            velocity.X = direction.X * speed.X;
        }

        public void Jump()
        {
            if (!isJumping && onGround)
            {
                velocity.Y = -500;
                isJumping = true;
            }
        }

        public virtual bool Collides(Entity e)
        {
            return CollisionBox.Intersects(e.CollisionBox);
        }

        public virtual bool Collides(Map map)
        {
            int x1 = CollisionBox.Left / map.TileSize;
            int y1 = CollisionBox.Top / map.TileSize;
            int x2 = CollisionBox.Right / map.TileSize;
            int y2 = CollisionBox.Bottom / map.TileSize - 1;

            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    if (map.GetTile(x, y).Color == Color.Red)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void Update(float elapsed)
        {
            velocity += gravity * elapsed;
            velocity += acceleration * elapsed;
            
            // Clamp Velocity
            velocity = Vector2.Clamp(velocity, new Vector2(-speed.X, -500), new Vector2(speed.X, 1000));

            //CheckCollisions();

            // THIS IS THE ACTUAL MOMENT WE MOVE THE ENTITY
            // NEED TO CHECK COLLISIONS BEFORE THIS...

            Vector2 prevPosition = position;
            position += velocity * elapsed;
            if (Collides(Map))
            {
                position = prevPosition;
            }
            
            // Clamp to Map
            
            position = Vector2.Clamp(position, Vector2.Zero, new Vector2(Map.Bounds.Width - Width, Map.Bounds.Height - Height));
            if (Y + Height == Map.Bounds.Height)
            {
                isJumping = false;
                onGround = true;
            }
        }

        private void CheckCollisions()
        {
            
            foreach (Entity e in Map.Entities)
            {
                if (e == this) continue;

                if (Collides(e))
                {
                    Color *= 0.999f;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color);

            // Debugging stuff
            spriteBatch.Draw(Util.BlankTexture, CollisionBox, Color.Red * 0.6f);
        }
    }
}
