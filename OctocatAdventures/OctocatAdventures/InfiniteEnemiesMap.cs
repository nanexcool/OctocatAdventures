using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public class InfiniteEnemiesMap : Map
    {
        float timer = 0;
        public float SecondsPerEnemy { get; set; }

        public InfiniteEnemiesMap(int width, int height) 
            : base(width, height)
        {
            SecondsPerEnemy = 2;
            timer = SecondsPerEnemy;
        }

        public override void Update(float elapsed)
        {
            timer += elapsed;
            if (timer >= SecondsPerEnemy)
            {
                AddEntity(new Entity(Util.Next(16, 33), Util.Next(32, 65))
                {
                    Color = Color.Blue,
                    Position = new Vector2(0, 400),
                    Velocity = new Vector2(100, 0)
                });
                timer -= SecondsPerEnemy;
            }
            base.Update(elapsed);
        }
    }
}
