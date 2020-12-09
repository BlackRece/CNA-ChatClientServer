using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
    public class GameControl : MonoGame.Forms.Controls.MonoGameControl {
        Client _client;
        Texture2D temp;

        public GameControl(Client client) {
            _client = client;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            temp = Editor.Content.Load<Texture2D>("img/RedShip");
        }

        protected void LoadContent() {
            string pathTo = Path.Combine(System.Environment.CurrentDirectory, "/Content/img/RedShip.png");

            using (var fileStream = new FileStream(pathTo, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                temp = Texture2D.FromStream(GraphicsDevice, fileStream);
            }

        }

        protected override void Update(GameTime gameTime) {
            // TODO: use this.Content to load your game content here
            base.Update(gameTime);
        }

        protected override void Draw() {
            // TODO: Add your drawing code here
            Editor.spriteBatch.Begin();
            Editor.spriteBatch.Draw(temp, new Vector2(0, 0), Color.White);
            Editor.spriteBatch.End();
            base.Draw();
        }

        protected override void Dispose(bool disposing) {
            if(temp != null & !temp.IsDisposed) {
                temp.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
