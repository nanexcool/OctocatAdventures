using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctocatAdventures
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Width * TileSize, Height * TileSize); }
        }

        public Vector2 Position { get; set; }

        public List<Entity> Entities { get; set; }

        public Tile[] Tiles { get; set; }
        public List<Tile> TilesToUpdate { get; set; }

        public const int TileSize = 32;

        public Map(int w, int h)
        {
            Width = w;
            Height = h;

            Entities = new List<Entity>();

            Tiles = new Tile[Width * Height];
            TilesToUpdate = new List<Tile>();

            InitializeMap();
        }

        public void InitializeMap()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Tiles[x + y * Width] = new Tile() 
                    {
                        Color = Color.Black * Util.NextFloat() 
                    };
                }
            }
            GetTile(0, 0).Color = Color.Red;
        }

        public Tile GetTile(int xTile, int yTile)
        {
            return Tiles[xTile + yTile * Width];
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);
            e.Map = this;
        }

        public virtual void Update(float elapsed)
        {
            foreach (Tile t in TilesToUpdate)
            {
                t.Update(elapsed);
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(elapsed);
                if (!Entities[i].Active)
                {
                    Entities.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.BlankTexture, new Rectangle(0, 0, Width * TileSize, Height * TileSize), Color.Goldenrod);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    spriteBatch.Draw(Util.BlankTexture, new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize), GetTile(x, y).Color);
                }
            }
            
            foreach (Entity e in Entities)
            {
                e.Draw(spriteBatch);
            }
        }
    }
}
