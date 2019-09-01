using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;

namespace DodgerCS {
    class Enemy { 
        private Texture2D texture;
        private Vector2 position;
        private int enemySpeed = 205;
        private bool isDead;
        private Random random;
        private int angle;
        private Rectangle collisionRect;

        public Enemy( Texture2D enemyTexture, Vector2 enemyPos ) {
            texture = enemyTexture;
            position = enemyPos;
            isDead = false;
            random = new Random();
            angle = random.Next( 1, 46 );
            collisionRect = new Rectangle( (int)enemyPos.X, (int)enemyPos.Y, enemyTexture.Width, enemyTexture.Height );
        }

        public bool Collision(Rectangle playerRect) {
            if ( playerRect.Intersects( collisionRect ) ) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update the enemy position and health status
        /// </summary>
        /// <param name="height">Height of the game window</param>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public void Update( int height, GameTime gameTime ) {
            collisionRect.X = (int)position.X;
            collisionRect.Y = (int)position.Y;

            position.Y += enemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if the enemy is out of window's height
            isDead = position.Y + texture.Height > height ? true : false;
        }

        /// <summary>
        /// Draw the enemy to the screen
        /// </summary>
        /// <param name="spriteBatch">Enable us to draw the sprite</param>
        public void Render( SpriteBatch spriteBatch ) {
            spriteBatch.Draw( texture, position, Color.White );
        }

        public void Unload() {
            texture.Dispose();
            texture = null;
        }

        /// <summary>
        /// Stop movement of enemy when colliding with the player
        /// </summary>
        public void StopMovement() {
            enemySpeed = 0;
            position = new Vector2( position.X, position.Y - 1 );
        }

        public Vector2 Pos {
            get { return position; }
        }

        public bool IsDead {
            get { return isDead; }
        }
    }
}
