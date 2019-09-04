using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using System;

namespace DodgerCS {
    /// <summary>
    /// Handle different game states
    /// </summary>
    enum GameState {
        MainMenu,
        Playing
    }

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

        private Texture2D startButton;
        private Texture2D exitButton;
        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;
        private Rectangle startButtonRect;
        private Rectangle exitButtonRect;

        private List<Enemy> enemies;
        private Random random;

        private SpriteFont scoreFont;
        private int playerScore;
        private float lastEnemySpawn;
        private float timeInsideBackgroundMusic;
        private int collisionCounter;
        private bool startOfGame;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GameState gameState;

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
            collisionCounter = 0;

            Content.RootDirectory = "Content";
        }

        private void RestartGame() {
            playerScore = 0;
            lastEnemySpawn = 0.0f;
            timeInsideBackgroundMusic = 0.0f;
            collisionCounter = 0;

            enemies.Clear();
            playerHit = false;

            MediaPlayer.Play( backgroundMusic );
            player.Pos = new Vector2( graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 );
            player.InitialMovement();
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
            startOfGame = true;
            gameState = GameState.MainMenu;
            startButtonPosition = new Vector2( ( graphics.PreferredBackBufferWidth / 2 ) - 50, ( graphics.PreferredBackBufferHeight / 2 ) - 30 );
            exitButtonPosition = new Vector2( ( graphics.PreferredBackBufferWidth / 2 ) - 50, ( graphics.PreferredBackBufferHeight / 2 ) - 5 );
            IsMouseVisible = true;

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
            backgroundMusic = Content.Load<Song>( "../Content/background" );
            gameOverMusic = Content.Load<Song>( "../Content/gameover" );

            startButton = Content.Load<Texture2D>( "../Content/start" );
            exitButton = Content.Load<Texture2D>( "../Content/exit" );

            // NOTE: We put the initialization of the player and enemy here
            // since we need to have the textures loaded in
            player = new Player( Content, new Vector2( graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 ) );
            enemy = new Enemy( enemyTexture, new Vector2( random.Next( 1, graphics.PreferredBackBufferWidth ), -enemyTexture.Height ) );
            enemies.Add( enemy );

            startButtonRect = new Rectangle( (int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height );
            exitButtonRect = new Rectangle( (int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height );

            MediaPlayer.Volume = 0.1f;
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
            startButton.Dispose();
            exitButton.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            currentKeyboardState = Keyboard.GetState();

            if ( gameState == GameState.Playing ) {
                if ( currentKeyboardState.IsKeyDown( Keys.Escape ) ) {
                    MediaPlayer.Stop();
                    gameState = GameState.MainMenu;
                }

                IsMouseVisible = false;
                if ( currentKeyboardState.IsKeyUp ( Keys.R ) && previousKeyboardState.IsKeyDown( Keys.R ) ) {
                    RestartGame();
                }

                if ( startOfGame ) {
                    MediaPlayer.Play( backgroundMusic );
                    startOfGame = false;
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

                // Spawn an enemy if 0.15 seconds has passed
                lastEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Update game entities
                player.Update( graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, gameTime );

                for ( int i = 0; i < enemies.Count; i++ ) {
                    enemies[i].Update( graphics.PreferredBackBufferHeight, gameTime );

                    // if the enemy collided with the player
                    if ( enemies[i].Collision( player.CollisionRect ) ) {
                        if ( playerHit && collisionCounter == 1 ) {
                            MediaPlayer.Stop();
                            MediaPlayer.Play( gameOverMusic );
                        }
                        playerHit = true;
                        collisionCounter += 1;
                        for ( int j = 0; j < enemies.Count; j++ ) {
                            enemies[j].StopMovement();
                        }
                        player.StopMovement();
                    }

                    if ( enemies[i].IsDead ) {
                        playerScore += 1;
                        enemies.RemoveAt( i );
                        i--;
                    }
                }
            }

            if ( gameState == GameState.MainMenu ) {
                if ( startButtonRect.Contains( Mouse.GetState().Position ) && Mouse.GetState().LeftButton == ButtonState.Pressed ) {
                    gameState = GameState.Playing;
                }
                
                if ( exitButtonRect.Contains( Mouse.GetState().Position ) && Mouse.GetState().LeftButton == ButtonState.Pressed ) {
                    Exit();
                }
            }
            
            base.Update(gameTime);

            previousKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear( Color.Black );

            spriteBatch.Begin();

            if ( gameState == GameState.Playing ) {
                spriteBatch.DrawString( scoreFont, "Press R to restart game", new Vector2( 0, 20 ), Color.White );
                DrawScore( spriteBatch );
                foreach ( Enemy enemy in enemies ) {
                    enemy.Render( spriteBatch );
                }

                player.Render( spriteBatch );
            }

            if ( gameState == GameState.MainMenu ) {
                IsMouseVisible = true;
                spriteBatch.Draw( startButton, startButtonPosition, Color.White );
                spriteBatch.Draw( exitButton, exitButtonPosition, Color.White );
            }
           
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
