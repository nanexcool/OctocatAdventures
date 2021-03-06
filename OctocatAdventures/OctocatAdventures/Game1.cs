using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OctocatAdventures
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyboardState, keyboardState;
        MouseState oldMouseState, mouseState;

        Map map;
        Player player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Util.Initialize(this);

            map = new InfiniteEnemiesMap(25, 15);

            map.AddEntity(new Player(Content.Load<Texture2D>("Octocat"))
            {
                Position = new Vector2(200, 100),
                Width = 100,
                Height = 82,
                Source = new Rectangle(0, 0, 100, 82)
            });

            player = (Player)map.Entities[0];

            Weapon.ReloadSound = Content.Load<SoundEffect>("sounds/gun_reload");
            //Weapon.EmptyShootSound = Content.Load<SoundEffectInstance>("sounds/gun_empty_click");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboardState.IsKeyDown(Keys.LeftControl) && oldKeyboardState.IsKeyUp(Keys.LeftControl))
            {
                IsFixedTimeStep = !IsFixedTimeStep;
                graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                graphics.ApplyChanges();
                Console.WriteLine(IsFixedTimeStep.ToString());
            }

            HandlePlayerInput();

            map.Update(elapsed);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            map.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandlePlayerInput()
        {
            if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                Tile t = map.GetTile(mouseState.X / map.TileSize, mouseState.Y / map.TileSize);
                if (t != null)
                {
                    if (t.Color != Color.Red)
                    {
                        t.PreviousColor = t.Color;
                        t.Color = Color.Red;
                    }
                    else
                        t.Color = t.PreviousColor;
                }
            }

            Vector2 d = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                d.X = -1;
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                d.X = 1;
            }
            if (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.OemComma))
            {
                player.Jump();
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {

            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.Shoot(Vector2.UnitX * -1);
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.Shoot(Vector2.UnitX);
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.Shoot(Vector2.UnitY * -1);
            }
            if (keyboardState.IsKeyDown(Keys.P) && oldKeyboardState.IsKeyUp(Keys.P))
            {
                player.Reload();
            }
            if (keyboardState.IsKeyDown(Keys.PageDown) && oldKeyboardState.IsKeyUp(Keys.PageDown))
            {
                player.ChangeWeapon();
            }

            player.Move(d);
        }
    }
}
