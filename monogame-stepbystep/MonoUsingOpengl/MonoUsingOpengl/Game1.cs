using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace MonoUsingOpengl
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D testSprite;

        private SoundEffect effect;
        private SoundEffect effectBackground;

        private Vector2 position = new Vector2(400, 240);

        int speedX = 1;
        int speedY = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            this.testSprite = Content.Load<Texture2D>("1.jpg");

            this.effect = Content.Load<SoundEffect>("explode");
            this.effect.Play();

            this.effectBackground = Content.Load<SoundEffect>("background");
            this.effectBackground.Play();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            Trace.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
            {
                //MediaPlayer.Play(effect);
                // do something here
                effect.Play();
            }

            if (position.X> GraphicsDevice.Viewport.Bounds.Width - testSprite.Width)
            {
                speedX = -1;
            }
            else if (position.X < 0)
            {
                speedX = 1;
            }

            if (position.Y > GraphicsDevice.Viewport.Bounds.Height - testSprite.Height)
            {
                speedY = -1;
            }
            else if (position.Y < 0)
                speedY = 1;

            position.X += speedX;
            position.Y += speedY;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(testSprite, position, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
