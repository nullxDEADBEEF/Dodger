using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;

namespace DodgerCS {
    class Player {
        private Texture2D texture;
        private Vector2 position;
        private KeyboardState keyboardState;
        private int playerSpeed = 200;
        private Rectangle collisionRect;

        public Player( ContentManager content, Vector2 playerPos ) {
            texture =  content.Load<Texture2D>( "../Content/player" );
            this.position = playerPos;
            collisionRect = new Rectangle( (int)playerPos.X, (int)playerPos.Y, texture.Width, texture.Height );
        }

        private void Collision( int width, int height ) {
            if ( position.X < 0 ) {
                position.X = 0;
            }

            if ( position.X + texture.Width > width ) {
                position.X = width - texture.Width;
            }

            if ( position.Y < 0 ) {
                position.Y = 0;
            }

            if ( position.Y + texture.Height > height ) {
                position.Y = height - texture.Height;
            }
        }

        public void Update( int width, int height, GameTime gameTime ) {
            collisionRect.X = (int)position.X;
            collisionRect.Y = (int)position.Y;
            Collision( width, height );
            keyboardState = Keyboard.GetState();
            KeyboardInput( gameTime );
        }

        public void Render( SpriteBatch spriteBatch ) {
            spriteBatch.Draw( texture, position, Color.White );
        }

        private void KeyboardInput( GameTime gameTime ) {
            if ( keyboardState.IsKeyDown( Keys.Left ) ) {
                position.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ( keyboardState.IsKeyDown( Keys.Right ) ) {
                position.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ( keyboardState.IsKeyDown( Keys.Up ) ) {
                position.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ( keyboardState.IsKeyDown( Keys.Down ) ) {
                position.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void StopMovement() {
            playerSpeed = 0;
        }

        public void Unload() {
            texture.Dispose();
            texture = null;
        }

        public Rectangle CollisionRect {
            get { return collisionRect; }
        }

        public Vector2 Pos {
            get {  return position; }
        }
    }
}
