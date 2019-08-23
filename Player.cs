using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DodgerCS {
    class Player {
        private Texture2D playerTexture;
        private Vector2 playerPos;
        MouseState mouseState;

        public Player( ContentManager content, Vector2 playerPos ) {
            playerTexture =  content.Load<Texture2D>( "../Content/player" );
            this.playerPos = playerPos;
        }

        public void Update( GameTime gameTime ) {
            mouseState = Mouse.GetState();
            MouseInput();
        }

        public void Render( SpriteBatch spriteBatch ) {
            spriteBatch.Draw( playerTexture, playerPos, Color.White );
        }


        public void MouseInput() {
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            playerPos = new Vector2( mouseX, mouseY );
        }

        public void Unload() {
            playerTexture.Dispose();
            playerTexture = null;
        }
    }
}
