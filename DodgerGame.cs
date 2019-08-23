using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace DodgerCS {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DodgerGame : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        SoundEffect backgroundMusic;
        SoundEffectInstance backgroundInstance;
        

        public DodgerGame() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            Window.Title = "Dodger";

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            player = new Player( Content, new Vector2( graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 ) );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundMusic = Content.Load<SoundEffect>( "../Content/background" );
            backgroundInstance = backgroundMusic.CreateInstance();
            backgroundInstance.IsLooped = true;
            backgroundMusic.Play( 0.1f, 0.0f, 0.0f );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            player.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if ( Keyboard.GetState().IsKeyDown( Keys.Escape ) )
                Exit();

            // TODO: Add your update logic here

            player.Update( gameTime );
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear( Color.Black );

            spriteBatch.Begin();
            player.Render( spriteBatch );
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
