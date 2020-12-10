using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Forms.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNA_Client {
    public class GameControler : MonoGameControl {
        public Client _client;
        Texture2D temp;

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            //Console.WriteLine("Current path: " + Editor.Content.RootDirectory);
            //Editor.Content.RootDirectory = "..\\..\\DesktopGL\\";
            //temp = Editor.Content.Load<Texture2D>("..\\..\\DesktopGL\\image\\RedShip");
            //temp = Editor.Content.Load<Texture2D>("image\\RedShip");
            LoadContent();
        }

        protected void LoadContent() {
            string pathTo = Path.Combine(
                System.Environment.CurrentDirectory,
                "Content\\image\\RedShip.png"
            );

            using (var fileStream = new FileStream(pathTo, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                temp = Texture2D.FromStream(GraphicsDevice, fileStream);
            }

        }

        protected override void Update(GameTime gameTime) {
            // TODO: use this.Content to load your game content here
            base.Update(gameTime);
        }

        protected override void Draw() {
            base.Draw();
            // TODO: Add your drawing code here
            Editor.spriteBatch.Begin();

            Editor.spriteBatch.DrawString(
                Editor.Font, _client._nick,
                new Vector2(
                    (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString(_client._nick).X / 2),
                    (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                    Color.White
                );

            if (temp != null) {
                Editor.spriteBatch.Draw(temp, new Vector2(0, 0), Color.White);
            }

            Editor.spriteBatch.End();
            
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(temp != null & !temp.IsDisposed) {
                temp.Dispose();
            }

        }
    }
}
