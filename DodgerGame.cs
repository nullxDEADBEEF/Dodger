using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using System;

namespace DodgerCS {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DodgerGame : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Player player;
        private bool playerHit;
        private Texture2D enemyTexture;
        private Enemy enemy;
        private Song backgroundMusic;
        private Song gameOverMusic;

        private List<Enemy> enemies;
        private Random random;

        private SpriteFont scoreFont;
        private int playerScore;
        private float lastEnemySpawn;
        private float timeInsideBackgroundMusic;

        private const float enemySpeed = 0.15f;
        private const float BACKGROUND_MUSIC_DELAY_OFFSET = 2.0f;

        public DodgerGame() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
            Window.Title = "Dodger";

            playerScore = 0;
            lastEnemySpawn = 0.0f;
            timeInsideBackgroundMusic = 0.0f;

            Content.RootDirectory = "Content";
        }

        private void DrawScore( SpriteBatch spriteBatch ) {
            spriteBatch.DrawString( scoreFont, "Score: " + playerScore, new Vector2( 0, 0 ), Color.White );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            enemies = new List<Enemy>();
            random = new Random();
            playerHit = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load content to the game
            scoreFont = Content.Load<SpriteFont>( "../Content/Score" );
            enemyTexture = Content.Load<Texture2D>( "../Content/baddie" );

            player = new Player( Content, new Vector2( graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 ) );
            enemy = new Enemy( enemyTexture, new Vector2( random.Next( 1, graphics.PreferredBackBufferWidth ), -enemyTexture.Height ) );
            enemies.Add( enemy );

            backgroundMusic = Content.Load<Song>( "../Content/background" );
            gameOverMusic = Content.Load<Song>( "../Content/gameover" );
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play( backgroundMusic );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            player.Unload();
            foreach( Enemy enemy in enemies ) {
                enemy.Unload();
            }
            gameOverMusic.Dispose();
            backgroundMusic.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if ( Keyboard.GetState().IsKeyDown( Keys.Escape ) ) {
                Exit();
            }

            // Play background music again if time exceeds the song duration
            timeInsideBackgroundMusic += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if ( timeInsideBackgroundMusic >= backgroundMusic.Duration.TotalSeconds - BACKGROUND_MUSIC_DELAY_OFFSET ) {
                MediaPlayer.Play( backgroundMusic );
                timeInsideBackgroundMusic = 0;
            }

            if ( lastEnemySpawn > enemySpeed && !playerHit ) {
                Enemy enemy = new Enemy( enemyTexture, new Vector2( random.Next( 1, graphics.PreferredBackBufferWidth ), -enemyTexture.Height ) );
                enemies.Add( enemy );
                lastEnemySpawn = 0.0f;
            }

            // TODO: press key for restarting the game....

            // Spawn an enemy if 0.15 seconds has passed
            lastEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update game entities
            player.Update( graphics.PreferredBackBufferWidth,
                           graphics.PreferredBackBufferHeight, gameTime );

            for ( int i = 0; i < enemies.Count; i++ ) {
                // if the enemy collided with the player
                if ( enemies[i].Collision( player.CollisionRect ) ) {
                    playerHit = true;
                    MediaPlayer.Stop();
                    MediaPlayer.Play( gameOverMusic );
                    for ( int j = 0; j < enemies.Count; j++ ) {
                        enemies[j].StopMovement();
                    }
                    player.StopMovement();
                }

                enemies[i].Update( graphics.PreferredBackBufferHeight, gameTime );

                if ( enemies[i].IsDead ) {
                    playerScore += 1;
                    enemies.RemoveAt( i );
                    i--;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear( Color.Black );

            spriteBatch.Begin();
            DrawScore( spriteBatch );
            foreach ( Enemy enemy in enemies ) {
                enemy.Render( spriteBatch );
            }

            player.Render( spriteBatch );
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
