using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    class Particle
    {
        //Represents the texture used for each particle
        private Texture2D texture;
 
        // Vector2 to represent x and y location on screen
        private Vector2 position;
 
        //Vector2 to represent x and y velocity
        private Vector2 velocity;
 
        //Vector2 to represent x and y acceleration
        private Vector2 acceleration;
 
        //Vector2 to represent the origin - used for rotation
        private Vector2 origin;
 
        //The scale factor of the particle
        private float scale;
 
        //The angle of rotation
        private float angle;
 
        //The speed of rotation
        private float angularVelocity;
 
        //The amount of time to live for each particle
        private float timeToLive;
 
        //The total time the particle has been living since creation
        private float timeSinceStart;
 
        //The color of the particle
        private Color color;
 
        //Property to check whether the total time since particle creation is less then its time to live
        public bool isAlive
        {
            get { return timeSinceStart < timeToLive ? true : false; }
        }

        //this does not need to be defined but for demonstration purposes I have
        public Particle() { }

        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration, Color color, float scale,
                   float angle, float angularVelocity, float timeToLive)
        {
            this.texture = Util.BlankTexture;
            this.position = position;
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.color = color;
            this.scale = scale;
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.timeToLive = timeToLive;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            timeSinceStart = 0.0f;
        }

        public void Update(float elapsed)
        {
            //add the angular velocity to the angle
            angle += angularVelocity * elapsed;

            //add the acceleration to the velocity
            velocity += acceleration * elapsed;

            //move the actual particle on screen by updating its position
            position += velocity * elapsed;

            // normalized lifetime is a value from 0 to 1 and represents how far
            // a particle is through its life. 0 means it just started, .5 is half
            // way through, and 1.0 means it's just about to be finished.
            // this value will be used to calculate alpha and scale, to avoid
            // having particles suddenly appear or disappear.
            float normalizedLifetime = timeSinceStart / timeToLive;

            // we want particles to fade in and fade out, so we'll calculate alpha
            // to be (normalizedLifetime) * (1-normalizedLifetime). this way, when
            // normalizedLifetime is 0 or 1, alpha is 0. the maximum value is at
            // normalizedLifetime = .5, and is
            // (normalizedLifetime) * (1-normalizedLifetime)
            // (.5)                 * (1-.5)
            // .25
            // since we want the maximum alpha to be 1, not .25, we'll scale the
            // entire equation by 4.
            float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
            color *= alpha;

            // make particles grow as they age. they'll start at 75% of their size,
            // and increase to 100% once they're finished.
            scale *= (.75f + .25f * normalizedLifetime);

            //add the time passed each frame to calculate its total time living
            timeSinceStart += elapsed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, new Rectangle((int)position.X, (int)position.Y, 4, 4), color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}
