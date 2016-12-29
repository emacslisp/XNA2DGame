using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LoadSprite
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D testSprite;
        private Texture2D shuttle;
        private Texture2D earth;
        private SoundEffect effect;
        private SoundEffect effect2;

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

            // TODO: use this.Content to load your game content here

            //System.IO.Stream stream = TitleContainer.OpenStream("Content/1.jpg");
            //background = Texture2D.FromStream(GraphicsDevice, stream);
            testSprite = Content.Load<Texture2D>("1.jpg"); // change these names to the names of your images
            //shuttle = Content.Load<Texture2D>("shuttle");  // if you are using your own images.
            //earth = Content.Load<Texture2D>("earth");

            this.effect = Content.Load<SoundEffect>("explode");
            this.effect.Play();

            //this.effect2 = Content.Load<Song>("background");
            //MediaPlayer.Play(effect2);
            this.effect2 = Content.Load<SoundEffect>("background");
            this.effect2.Play();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //@example: xna - handler key input
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
            {
                //MediaPlayer.Play(effect);
                // do something here
                effect.Play();
            }

            // TODO: Add your update logic here

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

            spriteBatch.Draw(testSprite, new Vector2(400, 240), Color.White);
            //spriteBatch.Draw(earth, new Vector2(400, 240), Color.White);
            //spriteBatch.Draw(shuttle, new Vector2(450, 240), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
