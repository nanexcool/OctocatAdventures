using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    class ParticleSystem : DrawableGameComponent
    {
        // the texture this particle system will use.
        private Texture2D texture;

        // this number represents the maximum number of effects this particle system
        // will be expected to draw at one time. this is set in the constructor and is
        // used to calculate how many particles we will need.
        private int howManyEffects;

        // the array of particles used by this system. these are reused, so that calling
        // AddParticles will not cause any allocations.
        Particle[] particles;

        // the queue of free particles keeps track of particles that are not curently
        // being used by an effect. when a new effect is requested, particles are taken
        // from this queue. when particles are finished they are put onto this queue.
        Queue<Particle> freeParticles;

        // returns the number of particles that are available for a new effect.
        //
        public int FreeParticleCount
        {
            get { return freeParticles.Count; }
        }

        //location where all the particles will emit from.
        private Vector2 emitterLocation;
        public Vector2 EmitterLocation
        {
            get { return emitterLocation; }
            set { emitterLocation = value; }
        }

        // minNumParticles and maxNumParticles control the number of particles that are
        // added when AddParticles is called. The number of particles will be a random
        // number between minNumParticles and maxNumParticles.
        private int minNumParticles;
        private int maxNumParticles;

        // this controls the texture that the particle system uses. It will be used as
        // an argument to ContentManager.Load.
        private string textureFilename;

        //minInitialSpeed and maxInitialSpeed are used to control the initial velocity
        // of the particles. The particle's initial speed will be a random number
        // between these two. The direction is determined by the function
        // PickRandomDirection, which can be overriden.
        private float minInitialSpeed;
        private float maxInitialSpeed;

        //minAcceleration and maxAcceleration are used to control the acceleration of
        // the particles. The particle's acceleration will be a random number between
        // these two. By default, the direction of acceleration is the same as the
        // direction of the initial velocity.
        private float minAcceleration;
        private float maxAcceleration;

        //minRotationSpeed and maxRotationSpeed control the particles' angular
        // velocity: the speed at which particles will rotate. Each particle's rotation
        // speed will be a random number between minRotationSpeed and maxRotationSpeed.
        // Use smaller numbers to make particle systems look calm and wispy, and large
        //numbers for more violent effects.
        private float minRotationSpeed;
        private float maxRotationSpeed;

        //minLifetime and maxLifetime are used to control the lifetime. Each
        // particle's lifetime will be a random number between these two. Lifetime
        // is used to determine how long a particle "lasts." Also, in the base
        // implementation of Draw, lifetime is also used to calculate alpha and scale
        // values to avoid particles suddenly "popping" into view
        private float minLifetime;
        private float maxLifetime;

        // to get some additional variance in the appearance of the particles, we give
        //them all random scales. the scale is a value between minScale and maxScale,
        // and is additionally affected by the particle's lifetime to avoid particles
        // "popping" into view.
        private float minScale;
        private float maxScale;

        //The type of Particle system
        private Type systemType;
        public Type SystemType
        {
            get { return systemType; }
            set { systemType = value; }
        }

        private Game game;

        public ParticleSystem(Game game, int howManyEffects)
            : base(game)
        {
            InitializeConstants();
            this.game = game;
        }

        public ParticleSystem(Game game, Type systemType, int howManyEffects, String textureFileName,
                int minNumberParticles, int maxNumberParticles,
                float minInitialSpeed, float maxInitialSpeed, float minAcceleration, float maxAcceleration,
                float minRotationSpeed, float maxRotationSpeed, int minLifetime, int maxLifetime,
                float minScale, float maxScale)
            : this(game, howManyEffects)//, textureFileName, minNumberParticles, maxNumberParticles)
        {
            this.systemType = systemType;
            this.minInitialSpeed = minInitialSpeed;
            this.maxInitialSpeed = maxInitialSpeed;
            this.minAcceleration = maxAcceleration;
            this.minRotationSpeed = minRotationSpeed;
            this.maxRotationSpeed = maxRotationSpeed;
            this.minLifetime = minLifetime;
            this.maxLifetime = maxLifetime;
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.game = game;
        }

        ///
        /// Set up the constants that will give this particle system its behavior and
        /// properties.
        ///
        public void InitializeConstants()
        {
            textureFilename = "explosion";

            systemType = Type.Exhaust;

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 40;
            maxInitialSpeed = 500;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosions should be relatively short lived
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = .3f;
            maxScale = 1.0f;

            minNumParticles = 20;
            maxNumParticles = 25;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            //blendState = BlendState.Additive;

            //DrawOrder = AdditiveDrawOrder;
        }

        /// Override the base class LoadContent to load the texture. once it's
        /// loaded, calculate the origin.
        ///
        protected override void LoadContent()
        {
            // make sure sub classes properly set textureFilename.
            if (string.IsNullOrEmpty(textureFilename))
            {
                string message = "textureFilename wasn't set properly";
                throw new InvalidOperationException(message);
            }
            // load the texture....
            texture = game.Content.Load<Texture2D>(textureFilename);

            base.LoadContent();
        }

        /// AddParticles's job is to add an effect somewhere on the screen. If there
        /// aren't enough particles in the freeParticles queue, it will use as many as
        /// it can. This means that if there not enough particles available, calling
        /// AddParticles will have no effect.
        ///
        ///where the particle effect should be created
        public void AddParticles(Vector2 where)
        {
            // the number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            int numParticles =
                Util.Next(minNumParticles, maxNumParticles);

            // create that many particles, if you can.
            int freeParticlesCount = freeParticles.Count;
            for (int i = 0; i < numParticles && freeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                Particle p = freeParticles.Dequeue();
                InitializeParticle(p);
            }
        }

        public virtual void InitializeParticle(Particle p)
       {
 
           //assign initial location to emitter location
           Vector2 where = emitterLocation;
 
           // first, call PickRandomDirection to figure out which way the particle
           // will be moving. velocity and acceleration's values will come from this.
           Vector2 direction = PickRandomDirection();
 
           // pick some random values for our particle
           float velocity =
               RandomBetween(minInitialSpeed, maxInitialSpeed);
           float acceleration =
               RandomBetween(minAcceleration, maxAcceleration);
           float lifetime =
               RandomBetween(minLifetime, maxLifetime);
           float scale =
               RandomBetween(minScale, maxScale);
           float rotationSpeed =
               RandomBetween(minRotationSpeed, maxRotationSpeed);
 
           // then initialize it with those random values. initialize will save those,
           // and make sure it is marked as active.
           //p.Initialize(velocity * direction, acceleration * direction, lifetime, scale, rotationSpeed);
       }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)Util.NextDouble() * (max - min);
        }

        private Vector2 PickRandomDirection()
        {
            float angle = RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
    enum Type
    {
        Explosion,
        Exhaust
    };
}
