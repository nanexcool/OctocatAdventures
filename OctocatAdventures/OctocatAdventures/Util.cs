using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public static class Util
    {
        public static Texture2D BlankTexture;
        public static SpriteFont Font;

        static Random random = new Random();

        public static void Initialize(Game game)
        {
            BlankTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            BlankTexture.SetData<Color>(new Color[] { Color.White });
            Font = game.Content.Load<SpriteFont>("Font");
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}
